using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common.DataModel;
using SCADA.Common.ScadaServices.Common;

namespace SCADA.Common.Proxies
{
    public class ScadaStorageProxy
    {
        private IScadaStorage proxy;

        public ScadaStorageProxy()
        {

            ChannelFactory<IScadaStorage> channelFactory = new ChannelFactory<IScadaStorage>
               (new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8033/IScadaStorage"));
            proxy = channelFactory.CreateChannel();

        }

        public void SetModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model)
        {
            proxy.SetModel(model);
        }

        public void SetTransactionModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model)
        {
            proxy.SetTransactionModel(model);
        }

        public void SetCimModel(Dictionary<DMSType, Container> model)
        {
            proxy.SetCimModel(model);
        }

        public void SetDomModel(List<SwitchingEquipment> model)
        {
            proxy.SetDomModel(model);
        }

        public Dictionary<Tuple<RegisterType, int>, BasePoint> GetModel()
        {
            return proxy.GetModel();
        }

        public Dictionary<Tuple<RegisterType, int>, BasePoint> GetTransactionModel()
        {
            return proxy.GetTransactionModel();
        }

        public Dictionary<DMSType, Container> GetCimModel()
        {
            return proxy.GetCimModel();
        }

        public List<SwitchingEquipment> GetDomModel()
        {
            return proxy.GetDomModel();
        }
        
        public void UpdateModel(Dictionary<Tuple<RegisterType, int>, BasePoint> updateModel)
        {
            proxy.UpdateModel(updateModel);
        }
    }
}
