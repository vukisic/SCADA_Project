using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Models;

namespace SCADA.Common.ScadaDb.Repositories
{
    public interface IDomRepository
    {
        void Add(List<DomDbModel> model);
        void AddOrUpdate(DomDbModel model);
        void AddOrUpdateRange(List<DomDbModel> list);
        void UpdateSingle(DomDbModel model);
        List<DomDbModel> GetAll();
    }
}
