using Caliburn.Micro;
using Simulator.Core.Model;
using Simulator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.ViewModels
{
    public class ControlWindowViewModel : Screen
    {
        private IMessageService _messageService;
        private Point _point;
        private event EventHandler<ClickEventArgs> _applyHandler;

        public Point Point 
        {
            get { return _point; }
            set { _point = value; NotifyOfPropertyChange(() => Point); }
        }

        public ControlWindowViewModel(Point point, EventHandler<ClickEventArgs> applyEvent, IMessageService messageService)
        {
            _messageService = messageService;
            _applyHandler = applyEvent;
            Point = point;
        }

        public void OnClick()
        {
            if(Validate(Point))
            {
                _applyHandler?.Invoke(this, new ClickEventArgs() { Point = Point });
                TryClose();
            }
            else
            {
                if(Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT || Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
                {
                    _messageService.ShowMessage($"Value {(Point as AnalogPoint).Value} is invalid for data type {Point.GroupId}");
                }
                else
                {
                    _messageService.ShowMessage($"Value {(Point as BinaryPoint).Value} is invalid for data type {Point.GroupId}");
                }
            }
            
        }

        #region Validation
        private bool Validate(Point point)
        {
            if (!ValudateValue(point))
                return false;

            if (Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT || Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
            {
                return ValidateAnalogValue((point as AnalogPoint).Value);
            }
            else
            {
                return ValidateBinaryValue((point as BinaryPoint).Value);
            }
        }

        private bool ValidateBinaryValue(int value)
        {
            return value == 1 || value == 0;
        }

        private bool ValidateAnalogValue(float value)
        {
            return value >= 0 && value <= 1000000;
        }

        private bool ValudateValue(Point point)
        {
            if (Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT || Point.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
            {
                return (point as AnalogPoint).Value >= 0 && (point as AnalogPoint).Value <= 1000000;
            }
            else
            {
                return (point as BinaryPoint).Value >= 0 && (point as BinaryPoint).Value <= 1000000;
            }
        }
        #endregion
    }
}
