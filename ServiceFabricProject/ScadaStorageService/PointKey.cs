using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace ScadaStorageService
{
    public class PointKey : IComparable<PointKey>, IEquatable<PointKey>
    {
        public Tuple<RegisterType,int> Value { get; set; }

        public PointKey()
        {

        }

        public PointKey(Tuple<RegisterType, int> value)
        {
            Value = value;
        }

        public int CompareTo(PointKey other)
        {
            if (this.Value.Item1 == other.Value.Item1 && this.Value.Item2 == other.Value.Item2)
                return 0;
            if (this.Value.Item1 == other.Value.Item1)
                return this.Value.Item2.CompareTo(other.Value.Item2);
            return -1;
        }

        public bool Equals(PointKey other)
        {
            return this.Value.Item1 == other.Value.Item1 && this.Value.Item2 == other.Value.Item2;
        }
    }
}
