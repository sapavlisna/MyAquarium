using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium.Config.Model
{
    public class Surface
    {
        public int TriggerPin { get; set; }
        public int EchoPin { get; set; }
        public int Samples { get; set; }
        public bool TurnedOn { get; set; }
        public int Interval { get; set; }
    }
}
