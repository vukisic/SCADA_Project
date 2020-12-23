using System;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NDS.FrontEnd;
using NDS.ServiceBus;
using NServiceBus;
using NServiceBus.Logging;
using SCADATransaction;

namespace NDS
{
    class Program
	{
		static void Main(string[] args)
		{
            AsyncMain().GetAwaiter().GetResult();
        }

        static readonly ILog log = LogManager.GetLogger<Program>();

        private static async Task AsyncMain()
        {
            Console.Title = "NetworkDynamicService";
            
            log.Info("SCADA started working..");

            var endpoint = await ServiceBusStartup.StartInstance()
                .ConfigureAwait(false);

            IFEP fep = new FEP();
            fep.updateEvent += Fep_updateEvent;
            // UnComment if you want to start FEP
            //fep.Start();
            Console.WriteLine("FEP Started!");

            // Command example: 
            // await endpoint.Send(new DemoCommand { DemoProperty = "Do something!" });
            // NOTE: Don't forget to add routes for each command in ServiceBusStartup! (you don't need to do this for events)

            // Event example:
            //await endpoint.Publish(new DemoEvent { DemoProperty = "Something happened!" })
            //    .ConfigureAwait(false);
            SCADAServer.instace = endpoint;
            SCADAServer scada = new SCADAServer();
            
            scada.OpenModel();
            scada.OpenTransaction();

            Console.ReadLine();
		}

        private static void Fep_updateEvent(object sender, UpdateArgs e)
        {
            // Update event handler
            // Use endpoint to publish
        }
    }
}
