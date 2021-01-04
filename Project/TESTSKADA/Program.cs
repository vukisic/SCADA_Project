using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NDS.FrontEnd;
using NDS.Proxies;
using NDS.ServiceBus;
using NServiceBus;
using NServiceBus.Logging;
using SCADA.Common;
using SCADA.DB.Models;
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
        static IEndpointInstance endpoint;
        private static System.Timers.Timer timer;

        private static async Task AsyncMain()
        {
            Console.Title = "NetworkDynamicService";
            
            log.Info("SCADA started working..");

            endpoint = await ServiceBusStartup.StartInstance()
                .ConfigureAwait(false);

            IFEP fep = new FEP();
            fep.updateEvent += Fep_updateEvent;
            // UnComment if you want to start FEP
           // fep.Start();
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

            //LoggingProxy proxy = new LoggingProxy();
            //proxy.Log(new SCADA.Common.Logging.LogEventModel() { EventType = SCADA.Common.Logging.LogEventType.INFO, Message = "Hello World!" });

            SetTimer();
            timer.Start();
            //timer.Dispose();

            Console.ReadLine();
		}

        private static void Fep_updateEvent(object sender, UpdateArgs e)
        {
            
            // Update event handler
            // Use endpoint to publish
        }
        private static void SetTimer()
        {
            timer = new System.Timers.Timer(30000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            HistoryProxy proxy = new HistoryProxy();
            proxy.Add(new HistoryDbModel { TimeStamp = DateTime.Now.ToString() });
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss}", e.SignalTime);
        }
    }
}
