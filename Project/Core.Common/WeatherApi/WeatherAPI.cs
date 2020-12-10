﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Core.Common.WeatherApi.Data;

namespace Core.Common.WeatherApi
{
    public class WeatherAPI
    {
        public List<double> GetResultsForNext6Hours()
        {
            List<double> results = new List<double>();
            try
            {
                HttpClient client = new HttpClient();
                var result = client.GetAsync("http://api.weatherapi.com/v1/forecast.json?key=f71ebcc49f3648b8807164839201012&q=Novi Sad&days=2").GetAwaiter().GetResult();
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
                    results.Add(weather.Forecast.ForecastDay.Days[0].Hour[i].GetPrecipValue());
                }
                // For next day
                if (results.Count < 6)
                {
                    for (int i = 0; i < 7 - results.Count; i++)
                    {
                        results.Add(weather.Forecast.ForecastDay.Days[1].Hour[i].GetPrecipValue());
                    }
                }

                return results;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }
    }
}
