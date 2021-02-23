using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SCADA.Common.DataModel;

namespace GUI.Converters
{
    //DO_REG RW
    //DI_REG R
    //IN_REG R
    //HR_INT RW

    public class PointTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is RegisterType)
            {
                RegisterType pt = (RegisterType)value;
                if (pt == RegisterType.BINARY_OUTPUT || pt == RegisterType.ANALOG_OUTPUT)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
