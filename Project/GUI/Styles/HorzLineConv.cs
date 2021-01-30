// source - https://tinyurl.com/y2yvob4r
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace GUI.Styles
{
    public class HorzLineConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TreeViewItem item = (TreeViewItem)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            int index = ic.ItemContainerGenerator.IndexFromContainer(item);

            if ((string)parameter == "left")
            {
                if (index == 0) // Either left most or single item
                    return (int)0;
                else
                    return (int)1;
            }
            else // assume "right"
            {
                if (index == ic.Items.Count - 1)    // Either right most or single item
                    return (int)0;
                else
                    return (int)1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
