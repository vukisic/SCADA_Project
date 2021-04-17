using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;
using FTN.Services.NetworkModelService.DataModel.Core;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkModelServiceSF
{
    internal sealed class NetworkModelServiceSF : StatefulService
    {
        private static NetworkModel _networkModel;
        public NetworkModelServiceSF(StatefulServiceContext context)
            : base(context)
        { _networkModel = new NetworkModel(StateManager); }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            var listeners =  new[] {
                new ServiceReplicaListener((context) =>
                {
                    string host = host = context.NodeContext.IPAddressOrFQDN;

                    EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndpointP");
                    int port = endpointConfig.Port;
                    string scheme = endpointConfig.Protocol.ToString();
                    string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/NetworkModelServiceSF", "net.tcp", host, port);
                    var listener = new WcfCommunicationListener<INetworkModelService>(
                        wcfServiceObject: new NetworkModelServiceProvider(this.StateManager, this.Context,ApplyDelta,GetValue,GetValues),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        address: new EndpointAddress(uri)
                    );
                    return listener;
                },"NetowrkModelServiceSF"),
                 new ServiceReplicaListener((context) =>
                {
                    string host = host = context.NodeContext.IPAddressOrFQDN;

                    EndpointResourceDescription endpointConfig = context.CodePackageActivationContext.GetEndpoint("ServiceEndpointTR");
                    int port = endpointConfig.Port;
                    string scheme = endpointConfig.Protocol.ToString();
                    string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/NetworkModelServiceSF", "net.tcp", host, port);
                    var listener = new WcfCommunicationListener<ITransactionStepsAsync>(
                        wcfServiceObject: new NetworkModelServiceTransactionProvider(this.Context, Prepare, Commit, Rollback),
                        serviceContext: context,
                        listenerBinding: new NetTcpBinding(SecurityMode.None),
                        address: new EndpointAddress(uri)
                    );
                    return listener;
                },"NetowrkModelServiceSFTransaction")
            };
            return listeners;
        } 

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _networkModel.Initialize();
        }

        public Task<UpdateResult> ApplyDelta(Delta delta)
        {
            return Task.FromResult(_networkModel.ApplyDelta(delta));
        }

        public Task<IdentifiedObject> GetValue(long globalId)
        {
            return _networkModel.GetValue(globalId);
        }

        public Task<List<IdentifiedObject>> GetValues(List<long> globalIds)
        {
            return _networkModel.GetValues(globalIds);
        }

        public Task<bool> Commit()
        {
            try
            {
                return _networkModel.Commit();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<bool> Prepare()
        {
            try
            {
                return _networkModel.Prepare();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task Rollback()
        {
            try
            {
                return _networkModel.Rollback();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
