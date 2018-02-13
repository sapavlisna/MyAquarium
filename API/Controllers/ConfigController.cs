using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aquarium;

namespace API.Controllers
{
    public class ConfigController : ApiController
    {
        private IConfigManager _configManager;

        public ConfigController(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        [HttpGet]
        public IConfig GetConfig()
        {
            return _configManager.GetConfig();
        }

        [HttpPost]
        public void UpdateConfig(IConfig config)
        {
            _configManager.SaveConfig(config);
        }
    }
}
