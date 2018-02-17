namespace Aquarium
{
    public interface IMainController
    {
        bool SetupArduinoConnection();
        void SetupLogger();
        void StartLights();
    }
}