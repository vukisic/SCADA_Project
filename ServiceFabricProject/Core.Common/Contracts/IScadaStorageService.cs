using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common.DataModel;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface IScadaStorageService
    {
        [OperationContract]
        Task SetModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model);
        [OperationContract]
        Task SetTransactionModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model);
        [OperationContract]
        Task SetCimModel(Dictionary<DMSType, Container> model);
        [OperationContract]
        Task SetDomModel(List<SwitchingEquipment> model);
        [OperationContract]
        Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> GetModel();
        [OperationContract]
        Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> GetTransactionModel();
        [OperationContract]
        Task<Dictionary<DMSType, Container>> GetCimModel();
        [OperationContract]
        Task<List<SwitchingEquipment>> GetDomModel();
        [OperationContract]
        Task UpdateModelValue(Dictionary<Tuple<RegisterType, int>, BasePoint> updateModel);
        [OperationContract]
        Task<BasePoint> GetSingle(RegisterType type, int index);
    }
}
