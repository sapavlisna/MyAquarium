using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aquarium.Config.Model
{
    public class LightState
    {
        public int ID { get; set; }
        public TimeSpan Time { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int LightIntensity { get; set; }
        [JsonIgnore]
        public LightDayStatus Status { get; set; }

        public LightState Copy()
        {
            var result = new LightState
            {
                ID = this.ID,
                Time = this.Time,
                DayOfWeek = this.DayOfWeek,
                LightIntensity = this.LightIntensity
            };

            return result;
        }
        
    }

    public enum LightDayStatus
    {
        Skip,
        Planned,
        Passed
    }
}
