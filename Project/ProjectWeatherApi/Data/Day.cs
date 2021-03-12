using System;
using System.Collections.Generic;

namespace ProjectWeatherApi.Data
{
    public class Day
    {
        public DateTime Date { get; set; }
        public List<HourObject> Hour { get; set; } = new List<HourObject>();
    }
}
