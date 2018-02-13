using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium
{
    public class MainController : IMainController
    {
        private ILogger _logger;
        private IConfigManager _configManager;
        private IArduinoComunication _arduinoComunication;
        private ILightManager _lightManager;

        public MainController(ILogger logger, IConfigManager configManager, IArduinoComunication arduinoComunication, ILightManager lightManager)
        {
            _logger = logger;
            _configManager = configManager;
            _arduinoComunication = arduinoComunication;
            _lightManager = lightManager;

            SetupLogger();
            if (SetupArduinoConnection())
            {
                StartLights();
            }
            else
            {
                throw new Exception("Error with arduino");
            }
        }

        public bool SetupArduinoConnection()
        {
            var serialPort = _arduinoComunication.FindSerialPort();
            return serialPort != null;
        }

        public void SetupLogger()
        {
            _logger.LogInfo = _configManager.GetConfig().LogInfo;
        }

        public void StartLights()
        {
            _lightManager.Run();
        }
    }
}
