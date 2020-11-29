namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class Breaker : ProtectedSwitch
    {
        public Breaker(long gID) : base(gID)
        {
        }
        public Breaker(Breaker breaker) : base(breaker)
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
