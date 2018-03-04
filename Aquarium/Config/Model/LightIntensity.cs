using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium.Config.Model
{
    public class LightIntensity
    {
        public IEnumerable<int> Pins { get; set; }
        public bool TurnedOn { get; set; }
        public int Interval { get; set; }
    }
}
