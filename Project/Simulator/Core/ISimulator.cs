using Simulator.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Core
{
    public interface ISimulator
    {
        void Start();
        void Stop();
        void LoadConfifg(Tuple<ushort,ushort,ushort,ushort> points);
        void UpdatePoint(ushort index, dnp3_protocol.dnp3types.eDNP3GroupID group, dnp3_protocol.tgttypes.eDataSizes dataSize, dnp3_protocol.tgtcommon.eDataTypes dataType, SingleInt32Union value);
        event EventHandler<UpdateEventArgs> updateEvent;
    }
}
