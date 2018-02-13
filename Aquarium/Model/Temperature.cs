using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium.Model
{
    public class Temperature
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string SensorId { get; set; }
        public double Value { get; set; }
    }
}
