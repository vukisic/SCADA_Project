using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.WeatherApi.Data
{
    public class Day
    {
        public DateTime Date { get; set; }
        public List<HourObject> Hour { get; set; } = new List<HourObject>();
    }
}
