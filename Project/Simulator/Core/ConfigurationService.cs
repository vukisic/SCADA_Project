using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Core
{
    public class ConfigurationService : IConfigurationChange
    {
        public void UpdateConfig(Tuple<ushort, ushort, ushort, ushort> points)
        {
            Simulator.UpdateConfig(points);
        }
    }
}
