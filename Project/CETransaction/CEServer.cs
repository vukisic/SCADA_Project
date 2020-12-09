using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace CETransaction
{
    public class CEServer
    {
        private ServiceHost modelServiceHost;
        private ServiceHost transactionServiceHost;

        public CEServer()
        {
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
            catch (Exception e)
            {

            }
        }

        public void CloseModel()
        {
            try
            {
                modelServiceHost.Close();
            }
            catch (Exception e)
            {

            }
        }

        public void OpenTransaction()
        {
            try
            {
                transactionServiceHost.Open();
            }
            catch (Exception e)
            {

            }
        }

        public void CloseTransaction()
        {
            try
            {
                transactionServiceHost.Close();
            }
            catch (Exception e)
            {

            }
        }
    }
}
