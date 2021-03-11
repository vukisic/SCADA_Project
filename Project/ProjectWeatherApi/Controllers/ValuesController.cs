using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProjectWeatherApi.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectWeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IConfiguration _configuration;
        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // GET api/local
        [HttpGet("{context}")]
        public ActionResult<IEnumerable<string>> Get(string context)
        {
            if (context != "local" && context != "actual")
                return BadRequest();
            List<double> result = context == "local" ? GetLocal() : GetActual();
            return Ok(result);
        }

        private List<double> GetLocal()
        {
            var result = _configuration.GetSection("Weather:Data").AsEnumerable().ToList();
            result.Reverse();
            return result.Take(6).Select(x => double.Parse(x.Value)).ToList();
        }

        private List<double> GetActual()
        {
            List<double> results = new List<double>();
            HttpClient client = new HttpClient();
            var result = client.GetAsync("http://api.weatherapi.com/v1/forecast.json?key=f71ebcc49f3648b8807164839201012&q=Berlin&days=2").GetAwaiter().GetResult();
            result.EnsureSuccessStatusCode();
            var r = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var weather = JsonConvert.DeserializeObject<ForecastResponse>(r);
            var hour = DateTime.Now.ToString("HH", CultureInfo.InvariantCulture);
            //Get current hour
            var current = weather.Forecast.ForecastDay.Days[0].Hour.Single(x => x.Time.ToString("HH", CultureInfo.InvariantCulture) == hour);
            var index = weather.Forecast.ForecastDay.Days[0].Hour.IndexOf(current);
            // For this day
            for (int i = index; i < 24; i++)
            {
                if (results.Count == 6)
                    break;
                results.Add(weather.Forecast.ForecastDay.Days[0].Hour[i].GetPrecipValue());
            }
            // For next day
            if (results.Count < 6)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (results.Count == 6)
                        break;
                    results.Add(weather.Forecast.ForecastDay.Days[1].Hour[i].GetPrecipValue());
                }
            }

            return results;
        }
    }
}
