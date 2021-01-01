﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Repositories;
using SCADA.Services.Providers;
using SCADA.Services.Common;

namespace SCADA.Services.Services
{
    public class DOMHost
    {
        private ServiceHost serviceHost;

        public DOMHost()
        {
            serviceHost = new ServiceHost(typeof(DOMProvider));
            serviceHost.AddServiceEndpoint(typeof(IDom), new NetTcpBinding(),
                new Uri("net.tcp://localhost:7029/IDom"));
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
