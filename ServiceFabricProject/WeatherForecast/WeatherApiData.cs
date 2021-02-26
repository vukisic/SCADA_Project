using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherForecast
{
    public partial class Temperatures
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public Current Current { get; set; }

        [JsonProperty("forecast")]
        public Forecast Forecast { get; set; }
    }

    public partial class Current
    {
        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("precip_mm")]
        public long PrecipMm { get; set; }

        [JsonProperty("uv")]
        public long Uv { get; set; }
    }

    public partial class Condition
    {
    }

    public partial class Forecast
    {
        [JsonProperty("forecastday")]
        public List<Forecastday> Forecastday { get; set; }
    }

    public partial class Forecastday
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("day")]
        public Day Day { get; set; }

        [JsonProperty("astro")]
        public Condition Astro { get; set; }

        [JsonProperty("hour")]
        public List<Hour> Hour { get; set; }
    }

    public partial class Day
    {
        [JsonProperty("totalprecip_mm")]
        public double TotalprecipMm { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }
    }

    public partial class Hour
    {
        [JsonProperty("time_epoch")]
        public long TimeEpoch { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("precip_mm")]
        public double PrecipMm { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("tz_id")]
        public string TzId { get; set; }

        [JsonProperty("localtime_epoch")]
        public long LocaltimeEpoch { get; set; }

        [JsonProperty("localtime")]
        public string Localtime { get; set; }
    }
}
