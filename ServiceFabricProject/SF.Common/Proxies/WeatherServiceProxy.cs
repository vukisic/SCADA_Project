using System.Collections.Generic;
using System.ServiceModel;
using Core.Common.WeatherApi;

namespace SF.Common.Proxies
{
    public class WeatherServiceProxy : ClientBase<IWeatherForecast>
    {
        public WeatherServiceProxy():base(new NetTcpBinding(),new EndpointAddress("net.tcp://localhost:27011/WeatherForecast"))
        {
        }

        public WeatherServiceProxy(string uri):base(new NetTcpBinding(),new EndpointAddress(uri))
        {
        }

        public List<double> GetForecast()
        {
            return this.Channel.GetForecast().GetAwaiter().GetResult();
        }
    }
}
