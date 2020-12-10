using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Common.WeatherApi.Data
{
    [JsonConverter(typeof(WeatherConverter))]
    public class ForecastDay
    {
        public List<Day> Days { get; set; } = new List<Day>();
    }
}
