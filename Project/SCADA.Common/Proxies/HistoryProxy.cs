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
    public class HistoryProxy
    {
        private IHistory proxy;

        public HistoryProxy()
        {

            ChannelFactory<IHistory> channelFactory = new ChannelFactory<IHistory>
               (new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:7017/IHistory"));
            proxy = channelFactory.CreateChannel();

        }

        public void Add(HistoryDbModel model)
        {
            proxy.Add(model);
        }

        public void AddRange(List<HistoryDbModel> list)
        {
            proxy.AddRange(list);
        }

        public List<HistoryDbModel> GetByTimestamp(DateTime timestamp)
        {
            return proxy.GetByTimestamp(timestamp);
        }

        public List<HistoryDbModel> GetInInverval(DateTime from, DateTime to)
        {
            return proxy.GetInInverval(from, to);
        }

        public List<HistoryDbModel> GetAll()
        {
            return proxy.GetAll();
        }

        public HistoryGraph GetGraph()
        {
            return proxy.GetGraph();
        }
    }
}
