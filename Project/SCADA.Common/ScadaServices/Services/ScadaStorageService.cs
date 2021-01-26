using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.ScadaDb.Access;
using SCADA.Common.ScadaDb.Providers;
using SCADA.Common.ScadaDb.Repositories;
using SCADA.Common.ScadaServices.Common;
using SCADA.Common.ScadaServices.Providers;

namespace SCADA.Common.ScadaServices.Services
{
    public class ScadaStorageService
    {
        private ServiceHost serviceHost;

        public ScadaStorageService()
        {
            serviceHost = new ServiceHost(typeof(ScadaStorageProvider));
            serviceHost.AddServiceEndpoint(typeof(IScadaStorage), new NetTcpBinding(),
                new Uri("net.tcp://localhost:8033/IScadaStorage"));
            serviceHost.Opening += ServiceHost_Opening;
        }

        private void ServiceHost_Opening(object sender, EventArgs e)
        {
            IReplicationRepository repo = new ReplicationRepository(new ScadaDbContext());
            var result = repo.Get();
            ScadaStorageProvider.Model = result;
            Console.WriteLine($"Readed from replica: {ScadaStorageProvider.Model.Count}");
        }

        public void Open()
        {
            try
            {
                serviceHost.Open();
            }
            catch (Exception e)
            {

            }
        }

        public void Close()
        {
            try
            {
                serviceHost.Close();
            }
            catch (Exception e)
            {

            }
        }
    }
}
