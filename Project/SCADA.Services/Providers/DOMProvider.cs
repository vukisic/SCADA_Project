using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Access;
using SCADA.DB.Models;
using SCADA.DB.Providers;
using SCADA.DB.Repositories;
using SCADA.Services.Common;

namespace SCADA.Services.Providers
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
