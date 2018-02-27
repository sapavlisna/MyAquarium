using System.Collections.Generic;
using Aquarium.Settings;

namespace Aquarium
{
    public interface IConfigLIghts
    {
        int LightMinValue { get; set; }
        int LightMaxValue { get; set; }
        int FullTurningOnMinutes { get; set; }
        bool UseSlowTurnOn { get; set; }
        int LightPinNumber { get; set; }
        IEnumerable<LightState> LightStates { get; set; }
    }
}