using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Aquarium.Model;

namespace Aquarium
{
    public class TempService
    {
        private IArduinoComunication _arduino;
        private ILogger _logger;
        private IConfigManager _config;
        private AquariumContext _dbContext;
        private Timer _timer;

        public TempService(IArduinoComunication arduino, ILogger logger, IConfigManager config, AquariumContext dbContext)
        {
            _arduino = arduino;
            _logger = logger;
            _config = config;
            _dbContext = dbContext;

            _timer = new Timer(_config.GetConfig().Temperature.Interval);
            _timer.Elapsed += _timer_Elapsed;
        }

        private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var resultString = GetTemp();
            var results = ParseResults(resultString);
            await SaveAsync(results);
        }

        private async Task SaveAsync(List<Temperature> results)
        {
            _dbContext.Temperature.AddRange(results);
            await _dbContext.SaveChangesAsync();
        }

        public void Run()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public List<Temperature> ParseResults(string result)
        {
            var records = result.Split(';');
            var resultList = new List<Temperature>();

            foreach(var record in records)
            {
                var values = record.Split('|');

                resultList.Add(new Temperature
                {
                    CreateDate = DateTime.Now,
                    SensorId = values[0],
                    Value = Double.Parse(values[1])
                });
            }

            return resultList;
        }

        private string GetTemp()
        {
           return _arduino.GetTemp();
        }
    }
}
