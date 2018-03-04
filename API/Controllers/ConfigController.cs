using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aquarium;
using Config.Model.Config;

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
        public Aquarium.Config.Model.Config GetConfig()
        {
            return _configManager.GetConfig();
        }

        [HttpPost]
        public void UpdateConfig(Aquarium.Config.Model.Config config)
        {
            _configManager.SaveConfig(config);
        }
    }
}
