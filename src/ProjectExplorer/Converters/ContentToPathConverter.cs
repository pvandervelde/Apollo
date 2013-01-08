//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Apollo.UI.Explorer.Converters
{
    /// <summary>
    /// Creates a non-rectangular path for a tab control.
    /// </summary>
    /// <source>
    /// http://stackoverflow.com/questions/561931/how-to-create-trapezoid-tabs-in-wpf-tab-control
    /// </source>
    [ExcludeFromCodeCoverage]
    public class ContentToPathConverter : IValueConverter
    {
        /// <summary>
        /// Stores a singleton instance of the converter.
        /// </summary>
        private static readonly ContentToPathConverter s_Singleton = new ContentToPathConverter();

        /// <summary>
        /// Gets the converter.
        /// </summary>
        public static ContentToPathConverter Value
        {
            get
            {
                return s_Singleton;
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var cp = (FrameworkElement)value;
            double h = cp.ActualHeight > 10 ? 1.4 * cp.ActualHeight : 10;
            double w = cp.ActualWidth > 10 ? 1.25 * cp.ActualWidth : 10;
            PathSegmentCollection ps = new PathSegmentCollection(4)
            {
                new LineSegment(new Point(1, 0.7 * h), true),
                new BezierSegment(new Point(1, 0.9 * h), new Point(0.1 * h, h), new Point(0.3 * h, h), true),
                new LineSegment(new Point(w, h), true),
                new BezierSegment(new Point(w + (0.6 * h), h), new Point(w + h, 0), new Point(w + (h * 1.3), 0), true)
            };

            return ps;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
