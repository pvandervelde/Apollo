using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using ProjectExplorerPrototype.Properties;
using ProjectExplorerPrototype.Projects;

namespace ProjectExplorerPrototype.Converters
{
    public sealed class ProjectProcessStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
            {
                throw new InvalidOperationException("The target must be an Image");
            }

            System.Drawing.Bitmap bitmap = null;
            switch ((ProcessState)value)
            {
                case ProcessState.None :
                    {
                        bitmap = Resources.Play_32;
                        break;
                    };
                case ProcessState.NotStarted:
                    {
                        bitmap = Resources.Play_32;
                        break;
                    };
                case ProcessState.Running: 
                    {
                        bitmap = Resources.Play_32;
                        break;
                    };
                case ProcessState.Finished: 
                    {
                        bitmap = Resources.Play_32;
                        break;
                    };
                case ProcessState.Invalidated:
                    {
                        bitmap = Resources.Play_32;
                        break;
                    };
                case ProcessState.Error :
                    {
                        bitmap = Resources.Play_32;
                        break;
                    };
                default:
                    throw new NotImplementedException();
            }
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
