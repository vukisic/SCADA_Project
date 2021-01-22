using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.DB.Utils;

namespace SCADA.DB.Repositories
{
    public interface IReplicationRepository
    {
        Dictionary<Tuple<RegisterType, int>, BasePoint> Get();
        void Set(List<BasePoint> points);
    }
}
