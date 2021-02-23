using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FTN.Common;
using System.Configuration;


namespace FTN.Services.NetworkModelService.DeltaDB
{
	public class DeltaDBInitializer : DropCreateDatabaseIfModelChanges<DeltaDBContext>
    {
        protected override void Seed(DeltaDBContext context)
        {
            base.Seed(context);
        }
    }
}
