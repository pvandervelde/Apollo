using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ProjectExplorerPrototype.Properties;

namespace ProjectExplorerPrototype.Converters
{
    public class FilterIsActiveToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(double))
            {
                throw new InvalidOperationException("The target must be an Image");
            }

            return (bool)value ? Settings.Default.FilterOpacity_Active : Settings.Default.FilterOpacity_NotActive;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
