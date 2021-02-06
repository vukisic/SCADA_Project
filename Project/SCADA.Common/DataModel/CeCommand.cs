using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.DataModel
{
    public class CeCommand
    {
        public RegisterType RegisterType { get; set; }
        public uint Index { get; set; }
        public uint Value { get; set; }
        public uint Milliseconds { get; set; }
        public uint MillisecondsPassedSinceLastPoll { get; set; }
        public bool Remove { get; set; }
    }
}
