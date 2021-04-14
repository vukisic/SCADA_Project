using Core.Common.Contracts;
using Core.Common.Json;
using Core.Common.PubSub;
using Core.Common.ServiceBus.Events;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SCADA.Common.DataModel;
using SF.Common.Proxies;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace NDSService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class NDSService : StatelessService
    {
        public NDSService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] {

            new ServiceInstanceListener((context) =>
            {
                string host = host = context.NodeContext.IPAddressOrFQDN;

                EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndPointNDSModel");

                int port = endpointConfig.Port;
                string scheme = endpointConfig.Protocol.ToString();
                string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/NDSService", "net.tcp", host, port);

                var listener = new WcfCommunicationListener<IModelUpdateAsync>(
                    wcfServiceObject: new NDSModelProvider(this.Context),
                    serviceContext: context,
                    listenerBinding: new NetTcpBinding(SecurityMode.None),
                    address: new EndpointAddress(uri)

                );
                return listener;
            },"NDSModelProvider"),

            new ServiceInstanceListener((context) =>
            {

                string host = host = context.NodeContext.IPAddressOrFQDN;

                EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndPointNDSTr");

                int port = endpointConfig.Port;
                string scheme = endpointConfig.Protocol.ToString();
                string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/NDSService", "net.tcp", host, port);

                var listener = new WcfCommunicationListener<ITransactionStepsAsync>(
                    wcfServiceObject: new NDSTransactionProvider(this.Context),
                    serviceContext: context,
                    listenerBinding: new NetTcpBinding(SecurityMode.None),
                    address: new EndpointAddress(uri)
                );
                return listener;
            },"NDSTransactionProvider")
        };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Update();
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
                catch (Exception ex)
                {
                    var log = new LogServiceProxy();
                    await log.Log(new SCADA.Common.Logging.LogEventModel()
                    {
                        EventType = SCADA.Common.Logging.LogEventType.ERROR,
                        Message = $"Message:{ex.Message}\nStackTrace:{ex.StackTrace}"
                    });
                }
            }
        }

        private async Task Update()
        {
            var domService = new DomServiceProxy();
            var historyService = new HistoryServiceProxy();
            var storageService = new ScadaStorageProxy();
            var domData = await domService.GetAll();
            DomUpdateEvent dom = new DomUpdateEvent()
            {
                DomData = domData.ToSwitchingEquipment()
            };
            HistoryUpdateEvent history = new HistoryUpdateEvent()
            {
                History = await historyService.GetAll()
            };
            HistoryGraphicalEvent graph = new HistoryGraphicalEvent()
            {
                Graph = await historyService.GetGraph()
            };
            ScadaUpdateEvent ev = new ScadaUpdateEvent()
            {
                Points = new List<SCADA.Common.DataModel.ScadaPointDto>()
            };
            
            var all = (await storageService.GetModel()).Values.ToList();
            var analogs = all.Where(x => x.RegisterType == RegisterType.ANALOG_INPUT || x.RegisterType == RegisterType.ANALOG_OUTPUT).Cast<AnalogPoint>().ToList();
            var binaries = all.Where(x => x.RegisterType == RegisterType.BINARY_INPUT || x.RegisterType == RegisterType.BINARY_OUTPUT).Cast<DiscretePoint>().ToList();
            ev.Points.AddRange(Mapper.MapCollection<AnalogPoint, ScadaPointDto>(analogs));
            ev.Points.AddRange(Mapper.MapCollection<DiscretePoint, ScadaPointDto>(binaries));

            Subscription subs = new Subscription();
            Publisher pub = new Publisher(subs.Topic, subs.ConnectionString);

            //if (ev.Points.Count > 0)
            //    pub.SendMessage(new PubSubMessage()
            //    {
            //        ContentType = ContentType.SCADA_UPDATE,
            //        Sender = Sender.SCADA,
            //        Content = JsonTool.Serialize<ScadaUpdateEvent>(ev)
            //    }).ConfigureAwait(false).GetAwaiter().GetResult();
            //if (dom.DomData.Count > 0)
            //    pub.SendMessage(new PubSubMessage()
            //    {
            //        ContentType = ContentType.SCADA_DOM,
            //        Sender = Sender.SCADA,
            //        Content = JsonTool.Serialize<DomUpdateEvent>(dom)
            //    }).ConfigureAwait(false).GetAwaiter().GetResult();
            //if (history.History.Count > 0)
            //    pub.SendMessage(new PubSubMessage()
            //    {
            //        ContentType = ContentType.SCADA_HISTORY,
            //        Sender = Sender.SCADA,
            //        Content = JsonTool.Serialize<HistoryUpdateEvent>(history)
            //    }).ConfigureAwait(false).GetAwaiter().GetResult();
            //pub.SendMessage(new PubSubMessage()
            //{
            //    ContentType = ContentType.SCADA_HISTORY_GRAPH,
            //    Sender = Sender.SCADA,
            //    Content = JsonTool.Serialize<HistoryGraphicalEvent>(graph)
            //}).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
