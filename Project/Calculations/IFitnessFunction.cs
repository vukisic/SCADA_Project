using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    public interface IFitnessFunction
    {
        void Compute(string test);
    }
}
