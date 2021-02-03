using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common.DataModel;

namespace SCADA.Common.ScadaServices.Common
{
    [ServiceContract]
    public interface IScadaStorage
    {
        [OperationContract]
        void SetModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model);
        [OperationContract]
        void SetTransactionModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model);
        [OperationContract]
        void SetCimModel(Dictionary<DMSType, Container> model);
        [OperationContract]
        void SetDomModel(List<SwitchingEquipment> model);
        [OperationContract]
        Dictionary<Tuple<RegisterType, int>, BasePoint> GetModel();
        [OperationContract]
        Dictionary<Tuple<RegisterType, int>, BasePoint> GetTransactionModel();
        [OperationContract]
        Dictionary<DMSType, Container> GetCimModel();
        [OperationContract]
        List<SwitchingEquipment> GetDomModel();
        [OperationContract]
        void UpdateModelValue(Dictionary<Tuple<RegisterType, int>, BasePoint> updateModel);
        [OperationContract]
        BasePoint GetSingle(RegisterType type, int index);
    }
}
