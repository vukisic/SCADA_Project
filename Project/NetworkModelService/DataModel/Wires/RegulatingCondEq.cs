using System.Runtime.Serialization;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    [DataContract]
    public class RegulatingCondEq : ConductingEquipment
    {
        public RegulatingCondEq(long gID) : base(gID)
        {
        }
        public RegulatingCondEq(RegulatingCondEq equipment) : base(equipment)
        {

        }

        public override bool Equals(object x)
        {
            return base.Equals(x);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
