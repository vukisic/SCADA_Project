using Core.Common.ServiceBus.Dtos;

namespace GUI.Models.Schema
{
    public class RatioTapChangerModel : BaseSchemaModel
    {
        public RatioTapChangerModel(RatioTapChangerDto tapChanger = null) : base(tapChanger)
        {
            HighStep = tapChanger?.HighStep ?? 0;
            LowStep = tapChanger?.LowStep ?? 0;
            NormalStep = tapChanger?.NormalStep ?? 0;
        }

        public int HighStep { get; set; }
        public int LowStep { get; set; }
        public int NormalStep { get; set; }
    }
}
