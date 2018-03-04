using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Aquarium.Classes;
using Aquarium.Model;
using Config.Model.Config;

namespace Aquarium.Services
{
    public class SurfaceService : ISurfaceService
    {
        private IArduinoService _arduinoService;
        private IConfigManager _configManager;
        private AquariumContext _aquariumContext;
        private System.Timers.Timer _timer;
        private int _interval;
        private bool _isRunning;

        public SurfaceService(IArduinoService arduinoService, IConfigManager configManager, AquariumContext aquariumContext)
        {
            this._arduinoService = arduinoService;
            this._configManager = configManager;
            this._aquariumContext = aquariumContext;

            _configManager.ConfigChanged += _configManager_ConfigChanged;
            SetTimerInterval();

            _timer = new System.Timers.Timer();
            _timer.Elapsed += timer_Elapsed;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var value = GetValue();
            SaveValue(value);
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

        public int GetValue()
        {
            var config = _configManager.GetConfig().Surface;
            return _arduinoService.GetDistance(config.TriggerPin, config.EchoPin, config.Samples);
        }

        public int SaveValue(int value)
        {
            var surface = new Surface
            {
                CteateDate = DateTime.Now,
                Value = value
            };

            _aquariumContext.Surface.Add(surface);
            return _aquariumContext.SaveChanges();
        }
    }
}
