using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Core.Common.WeatherApi.Data;
using Newtonsoft.Json;

namespace Core.Common.WeatherApi
{
    public class WeatherAPI
    {
        public WeatherAPI() { }
        public List<double> GetResultsForNext6Hours()
        {
            List<double> results = new List<double>();
            try
            {
                HttpClient client = new HttpClient();
                // LOCAL - https://localhost:44307/api/values/local
                // ACTUAL - https://localhost:44307/api/values/actual
                var result = client.GetAsync("https://localhost:44307/api/values/local").GetAwaiter().GetResult();
                result.EnsureSuccessStatusCode();
                var r = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                results = JsonConvert.DeserializeObject<double[]>(r).ToList();
                return results;
            }
            catch (Exception)
            {
                return new List<double>() {1.2,0,0,0,0,0};
            }

        }
    }
}
