//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;

namespace Apollo.ProjectExplorer.Converters
{
    /// <summary>
    /// Creates a margin.
    /// </summary>
    /// <source>
    /// http://stackoverflow.com/questions/561931/how-to-create-trapezoid-tabs-in-wpf-tab-control.
    /// </source>
    [ExcludeFromCodeCoverage]
    public class ContentToMarginConverter : IValueConverter
    {
        /// <summary>
        /// Stores a singleton instance of the converter.
        /// </summary>
        private static readonly ContentToMarginConverter s_Singleton = new ContentToMarginConverter();

        /// <summary>
        /// Gets the converter.
        /// </summary>
        public static ContentToMarginConverter Value
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
            return new Thickness(0, 0, -((FrameworkElement)value).ActualHeight, 0);
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
