//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Security;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Apollo.UI.Wpf.Properties;
using Microsoft.Win32.SafeHandles;

namespace Apollo.UI.Wpf.Converters
{
    /// <summary>
    /// Converts a boolean value indicating if a dataset is activated or not to an image that can be used
    /// to indicate a reversing of that state.
    /// </summary>
    public sealed class DatasetStateToImageConverter : IValueConverter
    {
        private sealed class SafeBitmapHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            private static extern bool DeleteObject(IntPtr hObject);

            [SecurityCritical]
            public SafeBitmapHandle(IntPtr preexistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                SetHandle(preexistingHandle);
            }

            protected override bool ReleaseHandle()
            {
                return DeleteObject(handle);
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isActivated = (bool)value;
            var img = isActivated ? Resources.Img_Deactivate_48 : Resources.Img_Activate_48;
            using (img)
            {
                using (var handle = new SafeBitmapHandle(img.GetHbitmap(), true))
                {
                    var result = Imaging.CreateBitmapSourceFromHBitmap(
                        handle.DangerousGetHandle(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    return result;
                }
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
