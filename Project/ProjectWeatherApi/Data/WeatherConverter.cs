using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProjectWeatherApi.Data
{
    public class WeatherConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType.Name.Equals("ForecastDay");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ForecastDay ret = new ForecastDay();
            JArray array = JArray.Load(reader);
            foreach (var item in array)
            {
                ret.Days.Add(item.ToObject<Day>());
            }
            return ret;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }
}
