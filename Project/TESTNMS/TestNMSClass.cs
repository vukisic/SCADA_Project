using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace TESTNMS
{
    public class TestNMSClass 
    {
        IEnlistManager proxy;
        public TestNMSClass() { }
        public void Initialize()
        {
            NetTcpBinding myBinding = new NetTcpBinding();
            EndpointAddress myEndpoint = new EndpointAddress("net.tcp://localhost:20000/TM");
            ChannelFactory<IEnlistManager> myChannelFactory = new ChannelFactory<IEnlistManager>(myBinding, myEndpoint);

            proxy = myChannelFactory.CreateChannel();           
        }

        public void SendModelToScada()
        {
            NetTcpBinding myBinding = new NetTcpBinding();
            EndpointAddress myEndpoint = new EndpointAddress("net.tcp://localhost:20006/SCADA");
            ChannelFactory<IModelUpdate> myChannelFactory = new ChannelFactory<IModelUpdate>(myBinding, myEndpoint);

            IModelUpdate _proxy = myChannelFactory.CreateChannel();

            // Call Service. 
            string pom = "marko";
            List<string> lista = new List<string>();
            Dictionary<string, List<string>> re = new Dictionary<string, List<string>>();
            re.Add(pom, lista);

            _proxy.UpdateModel(re);
        }

        public void EnList()
        {
            proxy.Enlist();
        }
        public void EndEnList(bool successed)
        {
            proxy.EndEnlist(successed);
        }
    }
}
