using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Models;
using SCADA.DB.Repositories;
using SCADA.Services.Common;

namespace NDS.Proxies
{
    public class DOMProxy
    {
        private IDom proxy;

        public DOMProxy()
        {

            ChannelFactory<IDom> channelFactory = new ChannelFactory<IDom>
               (new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:7002/IDom"));
            proxy = channelFactory.CreateChannel();

        }

        void AddOrUpdate(DomDbModel model)
        {
            proxy.AddOrUpdate(model);
        }
        void AddOrUpdateRange(List<DomDbModel> list)
        {
            proxy.AddOrUpdateRange(list);
        }
        void UpdateSingle(DomDbModel model)
        {
            proxy.UpdateSingle(model);
        }
        List<DomDbModel> GetAll()
        {
            return proxy.GetAll();
        }
    }
}
