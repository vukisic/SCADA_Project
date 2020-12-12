using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    [DataContract]
    public class ProtectedSwitch : Switch
    {
        public ProtectedSwitch(long gID) : base(gID)
        {
        }
        public ProtectedSwitch(ProtectedSwitch protectedSwitch) : base(protectedSwitch)
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
