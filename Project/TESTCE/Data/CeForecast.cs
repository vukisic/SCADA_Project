using System.Collections.Generic;

namespace CE.Data
{
    public class CeForecast
    {
        public List<CeForecastResult> Results { get; set; }

        public CeForecast()
        {
            Results = new List<CeForecastResult>();
        }
    }
}
