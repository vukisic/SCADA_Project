using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.WeatherApi;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class WeatherServiceProxy
    {
        public List<double> GetForecast()
        {
            Binding binding = WcfUtility.CreateTcpClientBinding();
            IServicePartitionResolver partitionResolver = ServicePartitionResolver.GetDefault();
            var wcfClientFactory = new WcfCommunicationClientFactory<IWeatherForecast>(clientBinding: binding, servicePartitionResolver: partitionResolver);
            var ServiceUri = new Uri("fabric:/ServiceFabricApp/WeatherForecast");
            var client = new WcfClient<IWeatherForecast>(wcfClientFactory, ServiceUri);
        
            var result = client.InvokeWithRetryAsync(x => x.Channel.GetForecast()).ConfigureAwait(false).GetAwaiter().GetResult();

            return result;
        }
    }
}
