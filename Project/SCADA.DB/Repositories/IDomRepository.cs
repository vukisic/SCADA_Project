using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Models;

namespace SCADA.DB.Repositories
{
    public interface IDomRepository
    {
        void AddOrUpdate(DomDbModel model);
        void AddOrUpdateRange(List<DomDbModel> list);
        void UpdateSingle(DomDbModel model);
        List<DomDbModel> GetAll();
    }
}
