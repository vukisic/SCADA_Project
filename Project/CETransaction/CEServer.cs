using System;
using System.Collections.Generic;
using System.ServiceModel;
using FTN.Common;
using FTN.Services.NetworkModelService;
using TMContracts;

namespace CETransaction
{
    public class CEServer
    {
        private ServiceHost modelServiceHost;
        private ServiceHost transactionServiceHost;
        public static EventHandler<Dictionary<DMSType, Container>> _pointUpdate = delegate { };
        public CEServer(EventHandler<Dictionary<DMSType, Container>> pointUpdate)
        {
            _pointUpdate = pointUpdate;
            modelServiceHost = new ServiceHost(typeof(CEModelProvider));
            modelServiceHost.AddServiceEndpoint(typeof(IModelUpdate), new NetTcpBinding(),
                new Uri("net.tcp://localhost:5002/IModelUpdate"));

            transactionServiceHost = new ServiceHost(typeof(CETransactionProvider));
            transactionServiceHost.AddServiceEndpoint(typeof(ITransactionSteps), new NetTcpBinding(),
                new Uri("net.tcp://localhost:4003/ITransactionSteps"));
        }

        public void OpenModel()
        {
            try
            {
                modelServiceHost.Open();
            }
            catch (Exception)
            {

            }
        }

        public void CloseModel()
        {
            try
            {
                modelServiceHost.Close();
            }
            catch (Exception)
            {

            }
        }

        public void OpenTransaction()
        {
            try
            {
                transactionServiceHost.Open();
            }
            catch (Exception)
            {

            }
        }

        public void CloseTransaction()
        {
            try
            {
                transactionServiceHost.Close();
            }
            catch (Exception)
            {

            }
        }
    }
}
