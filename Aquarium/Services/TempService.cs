using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Aquarium.Classes;
using Aquarium.Log;
using Aquarium.Model;
using Config.Model.Config;
using Timer = System.Timers.Timer;

namespace Aquarium
{
    public class TempService : ITempService
    {
        private IArduinoService _arduino;
        private ILogger _logger;
        private IConfigManager _config;
        private AquariumContext _dbContext;
        private Timer _timer;
        private int _interval;
        private bool _isRunning;

        public TempService(IArduinoService arduino, ILogger logger, IConfigManager config, AquariumContext dbContext)
        {
            _arduino = arduino;
            _logger = logger;
            _config = config;
            _dbContext = dbContext;

            _config.ConfigChanged += _config_ConfigChanged;
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
            SetTimerInterval();
        }

        private void _config_ConfigChanged(object sender, EventArgs e)
        {
            SetTimerInterval();
        }

        private void SetTimerInterval()
        {
            var interval = _config.GetConfig().Temperature.Interval * Constants.timeIntervalMultiplier;
            _interval = interval;

            if (interval != _interval && _isRunning)
            {
                this.Stop();
                this.Run();
            }
        }

        private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _logger.Write("Getting Temps", LoggerTypes.LogLevel.Info);
                var resultString = GetTemp();
                var results = ParseResults(resultString);
                await SaveAsync(results);
            }
            catch (Exception ex)
            {
                _logger.Write(ex);
            }
        }

        private async Task SaveAsync(List<Temperature> results)
        {
            try
            {
                _dbContext.Temperature.AddRange(results);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        public void Run()
        {
            _timer.Interval = _interval;
            _timer.Start();
            _timer_Elapsed(this, null);
            _isRunning = true;
        }

        public void Stop()
        {
            _timer.Stop();
            _isRunning = false;
        }

        private List<Temperature> ParseResults(string result)
        {
            var records = result.Split(';');
            var resultList = new List<Temperature>();

            foreach (var record in records)
            {
                try
                {
                    if (string.IsNullOrEmpty(record) || record == "\r\n")
                        continue;

                    var values = record.Split('|');

                    resultList.Add(new Temperature
                    {
                        CreateDate = DateTime.Now,
                        SensorId = values[0],
                        Value = Double.Parse(values[1].Replace('.', ','))
                    });
                }
                catch (Exception ex)
                {
                    _logger.Write($"Cannot insert temperature. Got data: {record}");
                }
            }

            return resultList;
        }

        private string GetTemp()
        {
            _logger.Write("Call arduino Getting Temps", LoggerTypes.LogLevel.Info);
            return _arduino.GetTemp(_config.GetConfig().Temperature.Pin);
        }
    }
}
