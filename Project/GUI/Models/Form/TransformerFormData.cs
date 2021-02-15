using Caliburn.Micro;
using GUI.Models.Schema;

namespace GUI.Models
{
    public class TransformerFormData : PropertyChangedBase
    {
        public TransformerFormData(TransformerModel transformer)
        {
            var tapChanger = transformer?.RatioTapChanger;
            HighStep = tapChanger?.HighStep ?? int.MaxValue;
            LowStep = tapChanger?.LowStep ?? 0;
            normalStep = tapChanger?.NormalStep ?? 0;
        }

        private int normalStep;

        public int NormalStep
        {
            get => normalStep;
            set
            {
                normalStep = value;
                NotifyOfPropertyChange(() => NormalStep);
            }
        }

        public int HighStep { get; }
        public int LowStep { get; }
    }
}
