using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Models;
using SCADA.Common.ScadaDb.Access;
using SCADA.Common.ScadaDb.Providers;
using SCADA.Common.ScadaDb.Repositories;
using SCADA.Common.ScadaServices.Common;

namespace SCADA.Common.ScadaServices.Providers
{
    public class DOMProvider : IDom
    {
        IDomRepository repo;
        public DOMProvider()
        {
            repo = new DomRepository(new ScadaDbContext());
        }
        public void AddOrUpdate(DomDbModel model)
        {
            repo.AddOrUpdate(model);
        }

        public void AddOrUpdateRange(List<DomDbModel> list)
        {
            repo.AddOrUpdateRange(list);
        }

        public List<DomDbModel> GetAll()
        {
            return repo.GetAll();
        }

        public void UpdateSingle(DomDbModel model)
        {
            repo.UpdateSingle(model);
        }
    }
}
