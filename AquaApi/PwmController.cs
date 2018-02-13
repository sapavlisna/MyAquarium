using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aquarium;

namespace AquaApi
{
    public class PwmController : ApiController
    {

        private IPwm _pwm { get; set; }

        public PwmController(IPwm pwm)
        {
            _pwm = pwm;
        }


        [HttpPost]
        public bool SetPwm([FromBody]PwmDTO pwmData)
        {
            return _pwm.SetPwm(pwmData.Pin, pwmData.Value);
        }

        [HttpGet]
        public string Test()
        {
            _pwm.SetPwm(9, 200);
            return "Ahoj";
        }
    }
}
