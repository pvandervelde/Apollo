using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using ProjectExplorerPrototype.Filters;
using ProjectExplorerPrototype.Properties;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;

namespace ProjectExplorerPrototype.Converters
{
    public class FilterIsActiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
            {
                throw new InvalidOperationException("The target must be an Image");
            }

            System.Drawing.Bitmap bitmap = (bool)value ? Resources.LightOn : Resources.LightOff;
            return GetBitmapSource(bitmap);
        }

        private BitmapSource GetBitmapSource(System.Drawing.Bitmap image)
        {
            System.Drawing.Bitmap bitmap = image;
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
