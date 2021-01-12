using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    public class GA
    {
        FluidLevelOptimization _levelOptimization;
        public GA(FluidLevelOptimization levelOptimization) {
            _levelOptimization = levelOptimization;
            //_levelOptimization.Compute("test");
        }
    }
}
