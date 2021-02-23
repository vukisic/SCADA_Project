// Source - https://tinyurl.com/y2yvob4r
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace GUI.Styles
{
    public class VertLineConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TreeViewItem item = (TreeViewItem)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            int index = ic.ItemContainerGenerator.IndexFromContainer(item);

            if ((string)parameter == "top")
            {
                if (ic is TreeView)
                    return 0;
                else
                    return 1;
            }
            else // assume "bottom"
            {
                if (item.HasItems == false)
                    return 0;
                else
                    return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
