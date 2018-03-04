using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aquarium
{
    public class Pwm : IPwm
    {
        private readonly IArduinoService _service;

        public Pwm(IArduinoService service)
        {
            this._service = service;
        }

        public bool SetPwm(int pin, int value)
        {
            _service.Write($"setpwm;{pin};{value};");
            var result = _service.Read();

            return result == "OK";
        }
    }
}
