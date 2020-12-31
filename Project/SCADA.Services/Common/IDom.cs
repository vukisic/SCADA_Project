using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Models;

namespace SCADA.Services.Common
{
    [ServiceContract]
    public interface IDom
    {
        [OperationContract]
        void AddOrUpdate(DomDbModel model);
        [OperationContract]
        void AddOrUpdateRange(List<DomDbModel> list);
        [OperationContract]
        void UpdateSingle(DomDbModel model);
        [OperationContract]
        List<DomDbModel> GetAll();
    }
}
