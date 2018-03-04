using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium.Config.Model
{
    public class Config
    {
        public ConfigLIghts LightConfig { get; set; }
        public Temperature Temperature { get; set; }
        public LightIntensity LightIntensity { get; set; }
        public Surface Surface { get; set; }
        public bool LogInfo { get; set; }
    }
}
