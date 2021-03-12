using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using FTN.Common;
using TMContracts;

namespace FTN.Services.NetworkModelService
{
	public class Program
	{
		
		private static void Main(string[] args)
		{
            Console.Title = "NMS";
			try
			{
                string message = "Starting Network Model Serivice...";
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
                Console.WriteLine("\n{0}\n", message);
                var serviceHost = new ServiceHost(typeof(TransactionProvider));
                serviceHost.AddServiceEndpoint(typeof(ITransactionSteps), new NetTcpBinding(), new Uri("net.tcp://localhost:4001/ITransactionSteps"));
                serviceHost.Open();
                
                using (NetworkModelService nms = new NetworkModelService())
                {
                    nms.Start();
                    message = "Press <Enter> to stop the service.";
                    CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
                    Console.WriteLine(message);
                    Console.ReadLine();
                }
               
               
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("NetworkModelService failed.");
				Console.WriteLine(ex.StackTrace);
				CommonTrace.WriteTrace(CommonTrace.TraceError, ex.Message);
				CommonTrace.WriteTrace(CommonTrace.TraceError, "NetworkModelService failed.");
				CommonTrace.WriteTrace(CommonTrace.TraceError, ex.StackTrace);
				Console.ReadLine();
			}
		}
	}
}
