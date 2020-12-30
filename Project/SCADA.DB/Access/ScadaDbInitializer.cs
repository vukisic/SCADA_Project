using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.DB.Access
{
    public class ScadaDbInitializer : DropCreateDatabaseIfModelChanges<ScadaDbContext>
    {
        protected override void Seed(ScadaDbContext context)
        {
            base.Seed(context);
        }
    }
}
