using System.Runtime.Serialization;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    [DataContract]
    public class Disconnector : Switch
    {
        public Disconnector(long gID) : base(gID)
        {
        }
        public Disconnector(Disconnector disconnector) : base(disconnector)
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
