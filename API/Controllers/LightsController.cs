using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aquarium;
using Aquarium.Settings;

namespace API.Controllers
{
    public class LightsController :ApiController
    {
        private ILightManager _lightManager;

        public LightsController()
        {
            
        }

        public LightsController(ILightManager lightManager)
        {
            _lightManager = lightManager;
        }

        [HttpPost]
        public void SetIntensity(int pin, int intensity)
        {
            _lightManager.SetIntensity(pin, intensity);
        }

        [Route("table")]
        [HttpGet]
        public IEnumerable<LightState> Get()
        {
            return _lightManager.GetLightTable();
        }
    }
}
