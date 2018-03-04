using System.IO.Ports;

namespace Aquarium
{
    public interface IArduinoService
    {
        bool IsConnected { get; }

        string ToString();
        SerialPort FindSerialPort();
        bool SetPWM(int pin, int value);
        void Write(string message);
        string Read();
        string GetTemp(int pin);
        int GetDistance(int triggerPin, int echoPin, int samples);
        int GetLightIntensity(int pin);
    }
}