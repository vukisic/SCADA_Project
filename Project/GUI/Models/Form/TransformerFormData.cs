using Caliburn.Micro;
using GUI.Models.Schema;
using System.Linq;

namespace GUI.Models
{
    public class TransformerFormData : PropertyChangedBase
    {
        public TransformerFormData(TransformerModel transformer)
        {
            var tapChanger = transformer?.RatioTapChanger;
            var measurement = tapChanger.Measurements.FirstOrDefault(m => m.MeasurementType == FTN.Common.MeasurementType.Discrete);

            Index = measurement?.Index;
            RegisterType = measurement?.RegisterType;

            MaxValue = (int)(measurement?.MaxValue ?? int.MaxValue);
            MinValue = (int)(measurement?.MinValue ?? 0);
            value = (int)(measurement?.Value ?? 0);
        }

        public SCADA.Common.DataModel.RegisterType? RegisterType { get; }
        public int? Index { get; }

        private int value;

        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }

        public int MaxValue { get; }
        public int MinValue { get; }
    }
}
