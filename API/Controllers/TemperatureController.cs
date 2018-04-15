using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aquarium;
using Aquarium.Model;

namespace API.Controllers
{
    public class TemperatureController : ApiController
    {
        private ITempService _tempService;

        public TemperatureController(ITempService tempService)
        {
            _tempService = tempService;
        }

        [HttpGet]
        public List<Temperature> GetTemps()
        {
            var tempsString = _tempService.GetTemp();
            var temps = _tempService.ParseResults(tempsString);

            return temps;
        }
    }
}
