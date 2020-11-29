using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FTN.Common;
using System.Configuration;


namespace FTN.Services.NetworkModelService.DeltaDB
{
	public class DeltaDBContext : DbContext
	{
        private DeltaDBInitializer deltaDBinitializer = new DeltaDBInitializer();

        public DeltaDBContext() : base("DeltaDBHistory")
        {
            Database.SetInitializer<DeltaDBContext>(deltaDBinitializer);
        }

        public DbSet<DeltaDBModel> Deltas { get; set; }
    }
}
