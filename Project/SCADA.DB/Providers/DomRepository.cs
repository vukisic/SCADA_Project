using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Access;
using SCADA.DB.Models;
using SCADA.DB.Repositories;

namespace SCADA.DB.Providers
{
    public class DomRepository : IDomRepository
    {
        private ScadaDbContext _context;

        public DomRepository(ScadaDbContext context) {
            _context = context;
        }

        public void AddOrUpdate(DomDbModel model)
        {
            throw new NotImplementedException();
        }

        public void AddOrUpdateRange(List<DomDbModel> list)
        {
            throw new NotImplementedException();
        }

        public List<DomDbModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public void UpdateSingle(DomDbModel model)
        {
            throw new NotImplementedException();
        }
    }
}
