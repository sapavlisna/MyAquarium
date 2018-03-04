using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium.Model
{
    public class LightIntensity
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public int Pin { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
