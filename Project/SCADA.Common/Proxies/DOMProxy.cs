using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Models;
using SCADA.Common.ScadaServices.Common;

namespace SCADA.Common.Proxies
{
    public class DOMProxy
    {
        private IDom proxy;

        public DOMProxy()
        {

            ChannelFactory<IDom> channelFactory = new ChannelFactory<IDom>
               (new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:7029/IDom"));
            proxy = channelFactory.CreateChannel();

        }

        public void AddOrUpdate(DomDbModel model)
        {
            proxy.AddOrUpdate(model);
        }
        public void AddOrUpdateRange(List<DomDbModel> list)
        {
            proxy.AddOrUpdateRange(list);
        }
        public void UpdateSingle(DomDbModel model)
        {
            proxy.UpdateSingle(model);
        }
        public List<DomDbModel> GetAll()
        {
            return proxy.GetAll();
        }
    }
}
