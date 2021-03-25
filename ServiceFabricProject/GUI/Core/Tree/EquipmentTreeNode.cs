using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using GUI.Models.Schema;

namespace GUI.Core.Tree
{
    public class EquipmentTreeNode : PropertyChangedBase
    {
        public ObservableCollection<EquipmentTreeNode> Children { get; set; } = new ObservableCollection<EquipmentTreeNode>();
        public string Name { get; set; } = "";

        private bool turnedOn = true;

        public bool TurnedOn
        {
            get { return turnedOn; }
            set
            {
                turnedOn = value;
                NotifyOfPropertyChange(() => TurnedOn);
                NotifyOfPropertyChange(() => ToolTip);
                NotifyOfPropertyChange(() => ValueText);
                NotifyOfPropertyChange(() => BorderColor);
            }
        }

        private bool active = true;

        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                NotifyOfPropertyChange(() => Active);
                NotifyOfPropertyChange(() => ToolTip);
                NotifyOfPropertyChange(() => ValueText);
                NotifyOfPropertyChange(() => BorderColor);
            }
        }

        private bool isClickable = true;

        public bool IsClickable
        {
            get { return isClickable; }
            set
            {
                isClickable = value;
                NotifyOfPropertyChange(() => IsClickable);
            }
        }

        private string imageSource = "";

        public string ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                NotifyOfPropertyChange(() => ImageSource);
            }
        }

        public Brush BorderColor => new SolidColorBrush(TurnedOn && Active ? Colors.Green : Colors.Red);

        public ICommand OnClick { get; set; }

        public string ToolTip => $"{Name}, {GetStatus()}";
        public string ValueText => $"{GetStatus()}";
        public string SpecialValue { get; set; }

        public ISchemaModel Item { get; set; }

        private string GetStatus()
        {
            return SpecialValue == null ? TurnedOn ? "ON" : "OFF" : SpecialValue;
        }
    }
}
