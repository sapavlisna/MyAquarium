using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Aquarium
{
    public class TempService
    {
        private IArduinoComunication _arduino;
        private ILogger _logger;
        private IConfigManager _config;
        private Timer _timer;

        public TempService(IArduinoComunication arduino, ILogger logger, IConfigManager config)
        {
            _arduino = arduino;
            _logger = logger;
            _config = config;

            _timer = new Timer(_config.GetConfig().Temperature.Interval);
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
        }

        public void Run()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void SaveTemp()
        {
            
        }

    }
}
