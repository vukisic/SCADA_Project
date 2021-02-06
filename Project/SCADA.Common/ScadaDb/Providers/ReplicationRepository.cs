using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.ScadaDb.Access;
using SCADA.Common.ScadaDb.Repositories;
using SCADA.Common.ScadaDb.Utils;

namespace SCADA.Common.ScadaDb.Providers
{
    public class ReplicationRepository : IReplicationRepository
    {
        private ScadaDbContext _context;

        public ReplicationRepository(ScadaDbContext context)
        {
            _context = context;
        }
        public Dictionary<Tuple<RegisterType, int>, BasePoint> Get()
        {
            var list = _context.ReplicationData.ToList();
            var converter = new ReplicationConverter();
            return converter.Convert(list);
        }

        public void Set(List<BasePoint> points)
        {
            _context.Database.ExecuteSqlCommand("TRUNCATE TABLE PointDbModels");
            _context.SaveChanges();
            var converter = new ReplicationConverter();
            var convertedPoints = converter.ConvertForDb(points);
            foreach (var item in convertedPoints)
            {
                _context.ReplicationData.Add(item);
            }
            _context.SaveChanges();
        }
    }
}
