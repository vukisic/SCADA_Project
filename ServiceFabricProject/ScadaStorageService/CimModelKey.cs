using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;

namespace ScadaStorageService
{
    public class CimModelKey: IComparable<CimModelKey>,IEquatable<CimModelKey>
    {
        public DMSType Value { get; set; }
        public CimModelKey(DMSType value)
        {
            Value = value;
        }
        public CimModelKey()
        {

        }

        public bool Equals(CimModelKey other)
        {
            return this.Value == other.Value;
        }

        public int CompareTo(CimModelKey other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}
