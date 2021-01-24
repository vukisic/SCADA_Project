using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.DB.Models;

namespace SCADA.DB.Access
{
    public class ScadaDbContext : DbContext
    {
        private ScadaDbInitializer deltaDBinitializer = new ScadaDbInitializer();

        public ScadaDbContext() : base("ScadaDB")
        {
            Database.SetInitializer<ScadaDbContext>(deltaDBinitializer);
        }

        public DbSet<HistoryDbModel> History { get; set; }
        public DbSet<DomDbModel> Dom { get; set; }
        public DbSet<PointDbModel> ReplicationData { get; set; }
    }
}
