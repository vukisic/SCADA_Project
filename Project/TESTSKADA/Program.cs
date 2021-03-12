using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NDS.FrontEnd;
using NDS.ServiceBus;
using NServiceBus;
using NServiceBus.Logging;
using SCADA.Common;
using SCADATransaction;
using NDS.Updaters;
using SCADA.Common.ScadaServices.Services;
using SCADA.Common.Proxies;
using NDS.ProcessingModule;

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
        private static async Task AsyncMain()
        {
            Console.Title = "NDS";
            LoggingHost logHost = new LoggingHost();
            logHost.Open();
            var logger = ScadaProxyFactory.Instance().LoggingProxy();
            log.Info("SCADA started working..");
            logger.Log(new SCADA.Common.Logging.LogEventModel() { EventType = SCADA.Common.Logging.LogEventType.INFO, Message = "SCADA NDS Started!" });

            endpoint = await ServiceBusStartup.StartInstance()
                .ConfigureAwait(false);

            SCADAServer.instace = endpoint;
            
            ScadaStorageService storage = new ScadaStorageService();
            storage.Open();

            SCADAServer scada = new SCADAServer();
            scada.OpenTransaction();
            scada.OpenModel();
            DOMHost dom = new DOMHost();
            dom.Open();

            HistoryHost historyHost = new HistoryHost();
            historyHost.Open();
            AlarmingKruncingHost ak = new AlarmingKruncingHost();
            ak.Open();

            ScadaExportService scadaExportService = new ScadaExportService();
            scadaExportService.Open();

            Console.WriteLine("Services are working..");
            logger.Log(new SCADA.Common.Logging.LogEventModel() { EventType = SCADA.Common.Logging.LogEventType.INFO, Message = "SCADA Services Started!" });

           

            GuiDBUpdater updater = new GuiDBUpdater(endpoint);
            updater.Start();

            IFEP fep = new FEP();
            Console.WriteLine("FEP Started!");
            logger.Log(new SCADA.Common.Logging.LogEventModel() { EventType = SCADA.Common.Logging.LogEventType.INFO, Message = "SCADA FEP Started!" });

            Console.ReadLine();
            storage.Close();
            ak.Close();
            dom.Close();
            logHost.Close();
            historyHost.Close();
            scadaExportService.Close();
            updater.Stop();
		}
    }
}
