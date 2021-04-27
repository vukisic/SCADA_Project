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
        public void SimulationSettings(bool enable)
        {
            Simulator.SimulationSettings(enable);
        }

        public void UpdateConfig(Tuple<ushort, ushort, ushort, ushort> points, Dictionary<string,ushort> pairs)
        {
            Simulator.UpdateConfig(points, pairs);
        }
    }
}
