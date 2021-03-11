using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProjectWeatherApi.Data
{
    [JsonConverter(typeof(WeatherConverter))]
    public class ForecastDay
    {
        public List<Day> Days { get; set; } = new List<Day>();
    }
}
