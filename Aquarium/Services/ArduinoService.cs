using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aquarium.Log;

namespace Aquarium
{
    public class ArduinoService : IArduinoService
    {
        public bool IsConnected { get { return _serialPort != null; } }
        public string ReadedData { get; private set; }

        private SerialPort _serialPort;
        private List<int> _baudRates = new List<int> { 115200, 19200, 230400, 38400, 4800, 57600, 9600 };
        private ILogger logger;

        private bool _isCommunicating = false;
        private object _communitationLockingObject = new object();

        private const string _endConstant = "END#";
        private const int _initWaitTime = 5000;
        private const int _readTimeout = 5000;
        private const int _writeTimeout = 5000;

        public ArduinoService(ILogger logger)
        {
            this.logger = logger;
        }

        public override string ToString()
        {
            return $"{_serialPort.PortName}:{_serialPort.BaudRate}";
        }

        public SerialPort FindSerialPort()
        {
            logger.Write("Finding serial port", LoggerTypes.LogLevel.System);
            var ports = new List<string>();

            logger.Write("Check environment", LoggerTypes.LogLevel.Info);

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                string[] ttys = Directory.GetFiles("/dev/", "tty*");

                foreach (string dev in ttys)
                {
                    if (dev.StartsWith("/dev/ttyUSB"))
                        ports.Add(dev);
                }
            }
            else
                ports = SerialPort.GetPortNames().Where(p => p.Contains("COM")).ToList();

            foreach (var port in ports)
            {
                logger.Write(port, LoggerTypes.LogLevel.Info);
            }


            foreach (var port in ports)
            {
                var connected = TryToConnect(port);
                if (connected)
                {

                    return _serialPort;
                }
            }

            return null;
        }

        private void OpenSerial()
        {
            _serialPort.Open();
            logger.Write($"Arduino restarted on opening port, waiting {_initWaitTime}ms.", LoggerTypes.LogLevel.System);
            Thread.Sleep(_initWaitTime);
        }

        public bool TryToConnect(string port)
        {
            logger.Write("Try baud rates", LoggerTypes.LogLevel.Info);
            _serialPort?.Dispose();

            _serialPort = new SerialPort(port);
            _serialPort.ReadTimeout = _readTimeout;
            _serialPort.WriteTimeout = _writeTimeout;

            foreach (var baudRate in _baudRates)
            {
                logger.Write($"Try to log to port {port} with baudrate {baudRate}", LoggerTypes.LogLevel.Info);
                _serialPort.BaudRate = baudRate;
                if (_serialPort.IsOpen)
                    _serialPort.Close();
                OpenSerial();


                if (_serialPort.IsOpen)
                    logger.Write("Port is opened", LoggerTypes.LogLevel.Info);
                else
                    logger.Write("Port is CLOSED", LoggerTypes.LogLevel.Info);


                try
                {
                    var result = WriteAndRead("info");

                    if (result.Contains("OK") || result.Contains("info"))
                    {
                        logger.Write($"Found Arduino", LoggerTypes.LogLevel.Info);
                        return true;
                    }
                    else
                    {
                        logger.Write($"Not found Arduino", LoggerTypes.LogLevel.Info);
                    }
                }
                catch (Exception e)
                {
                    logger.Write(e);
                    return false;
                }
            }

            return false;
        }

        public string WriteAndRead(string message)
        {
            while (true)
            {
                try
                {
                    Write(message);

                    var result = Read();
                    logger.Write($"Readed {result}", LoggerTypes.LogLevel.Info);
                    return result;
                }
                catch (Exception e)
                {
                    logger.Write(e);
                }
            }
        }

        public void Write(string message, SerialPort serial)
        {
            logger.Write($"Write to arduino: '{message}'", LoggerTypes.LogLevel.Info);

            if (!serial.IsOpen)
                return;

            serial.Write(message);
        }

        public void Write(string message)
        {
            Write(message, _serialPort);
        }

        public string Read()
        {
            var result = Read(_serialPort);

            return result;
        }

        private string Read(SerialPort serial)
        {
            var result = serial.ReadTo(_endConstant);
            if(result != "")
                logger.Write($"Read from arduino: '{result}'", LoggerTypes.LogLevel.Info);

            return result;
        }

        private void Lock()
        {
            int i = 0;

            logger.Write($"Locking serial port", LoggerTypes.LogLevel.Info);
            while (_isCommunicating == true && i < 10)
            {
                Thread.Sleep(200);
            }

            lock (_communitationLockingObject)
            {
                logger.Write($"Serial port is locked", LoggerTypes.LogLevel.Info);
                _isCommunicating = true;
            }
        }

        private void UnLock()
        {
            logger.Write($"Unlocking serial port", LoggerTypes.LogLevel.Info);
            lock (_communitationLockingObject)
            {
                _isCommunicating = false;
            }
        }


        public bool SetPWM(int pin, int value)
        {
            string result = "";

            try
            {
                Lock();
                logger.Write($"Setting PWM on pin {pin} with value {value}.", LoggerTypes.LogLevel.Info);
                Write($"setpwm;{pin};{value}");

                result = Read();
            }
            catch (Exception ex)
            {
                logger.Write(ex);
            }
            finally
            {
                UnLock();
            }

            return result == "OK";
        }

        public string GetTemp(int pin)
        {
            string result = "";
            try
            {
                logger.Write("Going to lock serial comunication", LoggerTypes.LogLevel.Info);
                Lock();
                logger.Write($"Reading temperatures.", LoggerTypes.LogLevel.Info);
                Write($"gettemps;{pin};");

                result = Read();
            }
            catch (Exception ex)
            {
                logger.Write(ex);
            }
            finally
            {
                UnLock();
            }

            return result;
        }

        public int GetLightIntensity(int pin)
        {
            try
            {
                Lock();
                logger.Write($"Reading light intensity on pin {pin}.", LoggerTypes.LogLevel.Info);
                Write($"getlight;{pin};");

                var result = Read();
                int value;

                var isParsed = int.TryParse(result, out value);

                if (isParsed)
                    return value;
            }
            catch (Exception ex)
            {
                logger.Write(ex);
            }
            finally
            {
                UnLock();
            }

            return -1;
        }

        public int GetDistance(int triggerPin, int echoPin, int samples)
        {
            int value = 0;
            bool isParsed = false;
            try
            {
                Lock();
                logger.Write(
                    $"Reading distance on trigger pin {triggerPin} and echo pin {echoPin} doing {samples} samples.",
                    LoggerTypes.LogLevel.Info);
                Write($"getdistance;{triggerPin};{echoPin};{samples};");

                int sleepTime;

                if (30 * samples < 500)
                    sleepTime = 500;
                else
                    sleepTime = 30 * samples;

                Thread.Sleep(sleepTime);
                var result = Read();
                isParsed = int.TryParse(result, out value);
            }
            catch (Exception ex)
            {
                logger.Write(ex);
            }
            finally
            {
                UnLock();
            }

            if (isParsed)
                return value;

            return -1;
        }
    }
}
