using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aquarium;
using Aquarium.Config.Model;

namespace API.Controllers
{
    public class LightsController :ApiController
    {
        private ILightControllService _lightControllService;

        public LightsController(ILightControllService lightControllService)
        {
            _lightControllService = lightControllService;
        }

        [HttpPost]
        public void SetIntensity(int pin, int intensity)
        {
            _lightControllService.SetIntensity(pin, intensity);
        }

        [Route("table")]
        [HttpGet]
        public IEnumerable<LightState> Get()
        {
            return _lightControllService.GetLightTable();
        }
    }
}
