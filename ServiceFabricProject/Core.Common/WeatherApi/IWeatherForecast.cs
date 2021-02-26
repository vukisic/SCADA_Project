using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.WeatherApi
{
    [ServiceContract]
    public interface IWeatherForecast
    {
        [OperationContract]
        Task<List<double>> GetForecast();
    }
}
