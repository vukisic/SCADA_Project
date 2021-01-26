using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.ScadaServices.Common;
using SCADA.Common.ScadaServices.Providers;

namespace SCADA.Common.ScadaServices.Services
{
    public class ScadaExportService
    {
        private ServiceHost _serviceHost;
        public ScadaExportService()
        {
            _serviceHost = new ServiceHost(typeof(ScadaExport));
            _serviceHost.AddServiceEndpoint(typeof(IScadaExport), new NetTcpBinding(),new Uri("net.tcp://localhost:21011/IScadaExport"));
        }

        public void Open()
        {
            try
            {
               _serviceHost.Open();
            }
            catch {}
        }

        public void Close()
        {
            try
            {
                _serviceHost.Close();
            }
            catch { }
        }
    }
}
