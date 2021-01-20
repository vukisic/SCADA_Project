using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculations;

namespace CE.Data
{
    public class CeForecastResult
    {
        public DNA<float> Result { get; set; }
        public int Count { get { return Result != null ? Result.Genes.Count() : 0; } }
        public float StartFluidLevel { get; set; }
        public float EndFluidLevel { get; set; }
        public List<float> Pumps { get;  set; }
        public List<float> Times { get;  set; }
        public List<float> Flows { get;  set; }

        public CeForecastResult()
        {
            Pumps = new List<float>();
            Flows = new List<float>();
            Times = new List<float>();
        }

    }
}
