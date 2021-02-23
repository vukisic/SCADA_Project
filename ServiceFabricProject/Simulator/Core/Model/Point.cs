using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Core.Model
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct SingleInt32Union
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public float f;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public int i;

        public SingleInt32Union(int val) : this()
        {
            i = val;
        }
    }
    public class Point
    {
        public dnp3_protocol.dnp3types.eDNP3GroupID GroupId { get; set; }
        public ushort Index { get; set; }
        public string TimeStamp { get; set; }
    }
}
