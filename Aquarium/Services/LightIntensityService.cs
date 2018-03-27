using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Aquarium.Classes;
using Aquarium.Log;
using Aquarium.Model;
using Config.Model.Config;

namespace Aquarium.Services
{
    public class LightIntensityService : ILightIntensityService
    {
        private IArduinoService _arduinoService;
        private IConfigManager _configManager;
        private ILogger _logger;
        private AquariumContext _aquariumContext;
        private Timer _timer;
        private int _interval;
        private bool _isRunning;

        public LightIntensityService(IArduinoService arduinoService, IConfigManager configManager, AquariumContext aquariumContext, ILogger logger)
        {
            this._arduinoService = arduinoService;
            this._configManager = configManager;
            this._aquariumContext = aquariumContext;
            this._logger = logger;

            _timer = new Timer();
            _timer.Elapsed += timer_Elapsed;
            _configManager.ConfigChanged += _configManager_ConfigChanged;
            SetTimerInterval();
        }

        private void _configManager_ConfigChanged(object sender, EventArgs e)
        {
            SetTimerInterval();
        }

        private void SetTimerInterval()
        {
            var interval = _configManager.GetConfig().Temperature.Interval * Constants.timeIntervalMultiplier;
            _interval = interval;

            if (interval != _interval && _isRunning)
            {
                this.Stop();
                this.Run();
            }
        }

        public void Run()
        {
            _timer.Interval = _interval;
            _timer.Start();
            timer_Elapsed(this, null);
            _isRunning = true;
        }

        public void Stop()
        {
            _timer.Stop();
            _isRunning = false;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var values = GetValues();
            SaveValues(values);
        }

        public int GetValue(int pin)
        {
            return _arduinoService.GetLightIntensity(pin);
        }

        public List<LightIntensity> GetValues()
        {
            var pins = _configManager.GetConfig().LightIntensity.Pins;

            var lightIntensityList = new List<LightIntensity>();

            foreach (var pin in pins)
            {
                var value = GetValue(pin);
                _logger.Write($"Measured light intensity {value}", LoggerTypes.LogLevel.Info);
                lightIntensityList.Add(new LightIntensity
                {
                    CreateDate = DateTime.Now,
                    Pin = pin,
                    Value = value
                }
                );
            }

            return lightIntensityList;
        }

        public int SaveValues(IEnumerable<LightIntensity> lightIntensities)
        {
            var a = lightIntensities.Count();
            foreach (var lightIntensity in lightIntensities)
            {
                _aquariumContext.LightInensity.Add(lightIntensity);
            }
            //_aquariumContext.LightInensity.AddRange(lightIntensities);
            return _aquariumContext.SaveChanges();
        }
    }
}
