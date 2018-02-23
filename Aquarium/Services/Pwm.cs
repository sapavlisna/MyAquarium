using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium
{
    public class Pwm : IPwm
    {
        private readonly IArduinoComunication _comunication;

        public Pwm(IArduinoComunication comunication)
        {
            this._comunication = comunication;
        }

        public bool SetPwm(int pin, int value)
        {
            _comunication.Write($"setpwm;{pin};{value};");

            var result = _comunication.Read();

            return result == "OK";
        }
    }
}
