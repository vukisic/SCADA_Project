using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnp3_protocol;

namespace Simulator.Core
{
    public interface ISimulatorConfiguration
    {
        dnp3types.sDNP3ConfigurationParameters Configure(string address, ushort port);
    }
}
