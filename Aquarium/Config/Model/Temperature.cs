using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium.Config.Model
{
    public class Temperature
    {
        public int Interval { get; set; }
        public int Pin { get; set; }
        public bool TurnedOn { get; set; }
    }
}