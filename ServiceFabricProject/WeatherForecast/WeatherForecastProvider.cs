using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Common.WeatherApi;
using Newtonsoft.Json;
using SF.Common;

namespace WeatherForecast
{
    public class WeatherForecastProvider : IWeatherForecast
    {
        private StatelessServiceContext _context;
        public WeatherForecastProvider(StatelessServiceContext context)
        {
            _context = context;
        }
        public async Task<List<double>> GetForecast()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "WeatherService called!");
            List<double> results = new List<double>();
            if (bool.TryParse(ConfigurationReader.ReadValue(_context, "Settings", "Debug"), out bool debug))
            {
                if (debug)
                    return new List<double>() { 1.2, 1.2, 1.2, 1.2, 1.2, 1.2 };
            }
            try
            {
                var apiKey = ConfigurationReader.ReadValue(_context, "Settings", "ApiKey");
                var location = ConfigurationReader.ReadValue(_context, "Settings", "Location");
                if (String.IsNullOrEmpty(apiKey) || String.IsNullOrEmpty(location))
                    return new List<double>() { 0, 0, 0, 0, 0, 0 };
                HttpClient client = new HttpClient();
                var result = await client.GetAsync(String.Format("http://api.weatherapi.com/v1/forecast.json?key={0}&q={1}&days=2", apiKey, location));
                result.EnsureSuccessStatusCode();
                var r = await result.Content.ReadAsStringAsync();
                var weather = JsonConvert.DeserializeObject<Temperatures>(r);
                var hour = DateTime.Now.ToString("HH", CultureInfo.InvariantCulture);
                var current = weather.Forecast.Forecastday[0].Hour.Single(x => DateTime.Parse(x.Time).ToString("HH", CultureInfo.InvariantCulture) == hour);
                var index = weather.Forecast.Forecastday[0].Hour.IndexOf(current);
                for (int i = index; i < 24; i++)
                {
                    if (results.Count == 6)
                        break;
                    results.Add(weather.Forecast.Forecastday[0].Hour[i].PrecipMm);
                }

                if (results.Count < 6)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        if (results.Count == 6)
                            break;
                        results.Add(weather.Forecast.Forecastday[1].Hour[i].PrecipMm);
                    }
                }
                return results;

            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(_context, $"EXCEPTION: {e.Message}");
                return null;
            }
        }
    }
}
