using System;
using System.Collections.Generic;

namespace Aquarium.Config.Model
{
    public class ConfigLIghts
    {
        public int LightMinValue { get; set; }
        public int LightMaxValue { get; set; }

        public int FullTurningOnMinutes { get; set; }
        public bool UseSlowTurnOn { get; set; }
        public int LightPinNumber { get; set; }
        
        public IEnumerable<LightState> LightStates { get; set; }
        public bool TurnedOn { get; set; }
    }
}