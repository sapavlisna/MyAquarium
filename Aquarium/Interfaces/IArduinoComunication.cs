using System.IO.Ports;

namespace Aquarium
{
    public interface IArduinoComunication
    {
        bool IsConnected { get; }

        string ToString();
        SerialPort FindSerialPort();
        bool SetPWM(int pin, int value);
        void Write(string message);
        string Read();
    }
}