using System;

namespace ProjectWeatherApi.Data
{
    public class HourObject
    {
        public DateTime Time { get; set; }
        public string Precip_mm { get; set; }

        public double GetPrecipValue()
        {
            if (double.TryParse(Precip_mm, out double result))
                return result;
            return 0;
        }
    }
}
