using Core.Common.Contracts;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SCADA.Common;
using SF.Common.Proxies;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace CommandingService
{
    internal sealed class CommandingService : StatefulService
    {
        public CommandingService(StatefulServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener((context) =>
                {
                    var listener = new WcfCommunicationListener<ICommandingServiceAsync>(
                        wcfServiceObject: new CommandingServiceProvider(this.Context, AddCommand),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        endpointResourceName: "ServiceEndpointCom"
                    );
                    return listener;

                })
            };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {

            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var commands = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<ScadaCommand>>>("commands");
                    using (var tx = this.StateManager.CreateTransaction())
                    {
                        var result = await commands.TryGetValueAsync(tx, "scada");
                        if (result.HasValue)
                        {
                            foreach (var item in result.Value)
                            {
                                if (item.MillisecondsPassedSinceLastPoll >= item.Milliseconds)
                                {
                                    // --- EXECUTE COMMAND --- 
                                    //processingManager.ExecuteWriteCommand(item.RegisterType, item.Index, item.Value);
                                    item.Remove = true;
                                }
                                item.MillisecondsPassedSinceLastPoll += 1000;
                            }
                            await commands.SetAsync(tx, "scada", result.Value.Where(x => x.Remove == false).ToList());
                        }
                        else
                        {
                            await commands.AddAsync(tx, "scada", new List<ScadaCommand>());
                        }
                        await tx.CommitAsync();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
                catch (Exception ex)
                {
                    await GetLogProxy().Log(new SCADA.Common.Logging.LogEventModel()
                    {
                        EventType = SCADA.Common.Logging.LogEventType.ERROR,
                        Message = $"Message:{ex.Message}\nStackTrace:{ex.StackTrace}"
                    });
                }
            }
        }

        public LogServiceProxy GetLogProxy()
        {
            return new LogServiceProxy();
        }

        public async Task AddCommand(ScadaCommand command)
        {
            var commands = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, List<ScadaCommand>>>("commands");

            using (var tx = this.StateManager.CreateTransaction())
            {
                if (await commands.GetCountAsync(tx) == 0)
                {
                    await commands.AddAsync(tx, "scada", new List<ScadaCommand>());
                }
                await tx.CommitAsync();
            }

            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await commands.TryGetValueAsync(tx, "scada");
                if (result.HasValue)
                {
                    result.Value.Add(command);
                }

                await commands.SetAsync(tx, "scada", result.Value);

                await tx.CommitAsync();
            }
        }

    }
}
