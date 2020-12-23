using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NServiceBus;
using SCADA.Common;
using TMContracts;

namespace SCADATransaction
{
    public class SCADAServer
    {
        private ServiceHost modelServiceHost;
        private ServiceHost transactionServiceHost;
        public static EventHandler updateEvent = new EventHandler(OnUpdateEvent);
        public static IEndpointInstance instace;


        public SCADAServer()
        {
            modelServiceHost = new ServiceHost(typeof(SCADAModelProvider));
            modelServiceHost.AddServiceEndpoint(typeof(IModelUpdate), new NetTcpBinding(), 

                new Uri("net.tcp://localhost:5001/IModelUpdate"));
            transactionServiceHost = new ServiceHost(typeof(SCADATransactionProvider));
            transactionServiceHost.AddServiceEndpoint(typeof(ITransactionSteps), new NetTcpBinding(), new Uri("net.tcp://localhost:4002/ITransactionSteps"));
        }

        public void OpenModel()
        {
            try
            {
                modelServiceHost.Open();
            }
            catch(Exception e)
            {

            }
        }
        
        public void CloseModel()
        {
            try
            {
                modelServiceHost.Close();
            }
            catch(Exception e)
            {

            }
        }
        
        public void OpenTransaction()
        {
            try
            {
                transactionServiceHost.Open();
            }
            catch(Exception e)
            {

            }
        }
        
        public void CloseTransaction()
        {
            try
            {
                transactionServiceHost.Close();
            }
            catch(Exception e)
            {

            }
        }

        private static void OnUpdateEvent(object sender, EventArgs e)
        {
            ScadaUpdateEvent ev = new ScadaUpdateEvent()
            {
                Points = DataBase.Model.Values.ToList()
            };
            try
            {
                instace.Publish(ev).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex) { }
        }
    }
}
