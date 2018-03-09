using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aquarium.Log;
using Aquarium.Services;
using Autofac;
using Config.Model.Config;

namespace Aquarium
{
    public class MainController : IMainController
    {
        private static IContainer Container { get; set; }

        private ILogger _logger;
        private IConfigManager _configManager;
        private IArduinoService _arduinoService;
        private ILightControllService _lightControllService;
        private ITempService _tempService;
        private ILightIntensityService _lightIntensityService;
        private ISurfaceService _surfaceService;

        public MainController()
        {
            try
            {
                SetupAutofac();
                using (var autofacScope = Container.BeginLifetimeScope())
                {
                    _configManager = autofacScope.Resolve<IConfigManager>();
                    _logger = autofacScope.Resolve<ILogger>();
                    _arduinoService = autofacScope.Resolve<IArduinoService>();
                    _lightControllService = autofacScope.Resolve<ILightControllService>();
                    _tempService = autofacScope.Resolve<ITempService>();
                    _lightIntensityService = autofacScope.Resolve<ILightIntensityService>();
                    _surfaceService = autofacScope.Resolve<ISurfaceService>();
                }

                _logger.Write("Starting services.", LoggerTypes.LogLevel.System);
                if (SetupArduinoConnection())
                {
                    StartLightController();
                    StartMeasureTemp();
                    StartMeasureLightIntensity();
                    StartMeasureSurface();
                    _logger.Write("Starting services finished.", LoggerTypes.LogLevel.System);
                }
                else
                {
                    throw new Exception("Error with arduino");
                }
            }
            catch (Exception ex)
            {
                var logger = new Logger();
                logger.Write(ex);
            }
        }

        private void StartMeasureLightIntensity()
        {
            if (_configManager.GetConfig().LightIntensity.TurnedOn)
            {
                _logger.Write("Starting light intensity measure service.", LoggerTypes.LogLevel.System);
                _lightIntensityService.Run();
            }
            else
                _logger.Write("Skip light intensity measure service.", LoggerTypes.LogLevel.System);
        }

        private void StartMeasureSurface()
        {
            if (_configManager.GetConfig().Surface.TurnedOn)
            {
                _logger.Write("Starting surface measure service.", LoggerTypes.LogLevel.System);
                _surfaceService.Run();
            }
            else
                _logger.Write("Skip surface measure service.", LoggerTypes.LogLevel.System);
        }

        private void StartMeasureTemp()
        {
            if (_configManager.GetConfig().Temperature.TurnedOn)
            {
                _logger.Write("Starting temperature measure service.", LoggerTypes.LogLevel.System);
                _tempService.Run();
            }
            else
                _logger.Write("Skip temperature measure service.", LoggerTypes.LogLevel.System);
        }

        public void StartLightController()
        {
            if (_configManager.GetConfig().LightConfig.TurnedOn)
            {
                _logger.Write("Starting light control service.", LoggerTypes.LogLevel.System);
                _lightControllService.Run();
            }
            else
                _logger.Write("Skip light control service.", LoggerTypes.LogLevel.System);
        }

        private void SetupAutofac()
        {
            //Logger.Write("Setup Autofac", LoggerTypes.LogLevel.Info);
            Console.WriteLine("Setup Autofac");
            var builder = new ContainerBuilder();
            builder.RegisterType<Model.AquariumContext>().SingleInstance();
            builder.RegisterType<ArduinoService>().As<IArduinoService>().SingleInstance();
            builder.RegisterType<ConfigManager>().As<IConfigManager>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            builder.RegisterType<LightControllService>().As<ILightControllService>().SingleInstance();
            builder.RegisterType<TempService>().As<ITempService>().SingleInstance();
            builder.RegisterType<LightIntensityService>().As<ILightIntensityService>();
            builder.RegisterType<SurfaceService>().As<ISurfaceService>();


            Container = builder.Build();
            //Logger.Write("Setup Autofac completed", LoggerTypes.LogLevel.Info);
            Console.WriteLine("Autofac setup completed");
        }

        public bool SetupArduinoConnection()
        {
            var serialPort = _arduinoService.FindSerialPort();
            return serialPort != null;
        }

        public void SetupLogger()
        {
            _logger.LogInfo = _configManager.GetConfig().LogInfo;
        }


    }
}
