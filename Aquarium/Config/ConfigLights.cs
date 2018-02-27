using System;
using System.Collections.Generic;
using Aquarium.Settings;

namespace Aquarium
{
    public class ConfigLIghts : IConfigLIghts
    {
        public int LightMinValue { get; set; }
        public int LightMaxValue { get; set; }

        public int FullTurningOnMinutes { get; set; }
        public bool UseSlowTurnOn { get; set; }
        public int LightPinNumber { get; set; }
        
        public IEnumerable<LightState> LightStates { get; set; }
    }
}