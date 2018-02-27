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
        public AquariumContext(string connectionString) :base(connectionString)
        {
            
        }

        public AquariumContext() : base()
        {
            
        }

        public DbSet<Temperature> Temperature { get; set; }
    }
}
