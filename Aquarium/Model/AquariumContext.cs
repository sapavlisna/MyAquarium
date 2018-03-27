using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aquarium.Model
{

    public class AquariumContext : DbContext
    {
        public AquariumContext()
        {
        }
        public DbSet<Temperature> Temperature { get; set; }
        public DbSet<LightIntensity> LightInensity { get; set; }
        public DbSet<Surface> Surface { get; set; }
    }
}
