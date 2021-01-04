﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Services.Common;
using SCADA.Services.Providers;

namespace SCADA.Services.Services
{
    public class HistoryHost
    {
        private ServiceHost serviceHost;

        public HistoryHost()
        {
            serviceHost = new ServiceHost(typeof(HistoryProvider));
            serviceHost.AddServiceEndpoint(typeof(IHistory), new NetTcpBinding(),
                new Uri("net.tcp://localhost:7017/IHistory"));
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
