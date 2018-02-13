using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aquarium.Settings;

namespace Aquarium
{
    public class Config : IConfig
    {
        public  ConfigLIghts LightConfig { get; set; }
        public Temperature Temperature { get; set; }
        public  bool LogInfo { get; set; }
    }
}
