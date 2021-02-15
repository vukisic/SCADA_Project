using Core.Common.ServiceBus.Dtos;

namespace GUI.Models.Schema
{
    public class TransformerModel : BaseSchemaModel
    {
        public TransformerModel()
        {
        }

        public TransformerModel(TransformerWindingDto winding, RatioTapChangerDto tapChanger = null, PowerTransformerDto powerTransformer = null)
            : base(winding)
        {
            PowerTransformer = new PowerTransformerModel(powerTransformer);
            RatioTapChanger = new RatioTapChangerModel(tapChanger);
        }

        public PowerTransformerModel PowerTransformer { get; set; }
        public RatioTapChangerModel RatioTapChanger { get; set; }

        public override void UpdateMeasurements(BasePointDto newMeasurement)
        {
            if (newMeasurement.ObjectMrid == PowerTransformer.MRID)
            {
                PowerTransformer.UpdateMeasurements(newMeasurement);
                return;
            }

            if (newMeasurement.ObjectMrid == RatioTapChanger.MRID)
            {
                RatioTapChanger.UpdateMeasurements(newMeasurement);
                return;
            }

            base.UpdateMeasurements(newMeasurement);
        }
    }
}
