using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using NDS.Proxies;
using NServiceBus;
using SCADA.DB.Providers;
using SCADA.DB.Repositories;

namespace NDS.Updaters
{
    public class GuiDBUpdater : IDisposable
    {
        private Thread worker;
        private IEndpointInstance endpoint;
        private bool executionFlag;
        private IDomRepository domRepo;

        public GuiDBUpdater(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
            this.domRepo = new DomRepository(new SCADA.DB.Access.ScadaDbContext());
        }

        public void Start()
        {
            this.worker = new Thread(DoWork);
            executionFlag = true;
            this.worker.Name = "Update thread";
            worker.Start();
        }

        private void DoWork()
        {
            while (executionFlag)
            {
                DomUpdateEvent dom = new DomUpdateEvent()
                {
                    DomData = ScadaProxyFactory.Instance().DOMProxy().GetAll().ToSwitchingEquipment()
                };
                if(dom.DomData.Count > 0)
                    endpoint.Publish(dom).ConfigureAwait(false).GetAwaiter().GetResult();
                Thread.Sleep(GetConfigTime());
            }
        }

        private int GetConfigTime()
        {
            return ConfigurationManager.AppSettings["updateInterval"] == null ? 10000 : Int32.Parse(ConfigurationManager.AppSettings["updateInterval"]);
        }

       public  void Stop()
        {
            this.executionFlag = false;
            this.worker.Abort();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
