//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace Apollo.UI.Wpf.Converters
{
    /// <summary>
    /// Converts a <see cref="RoutedUICommand"/> to a tool tip text representation.
    /// </summary>
    public sealed class CommandToToolTipTextConverter : IValueConverter
    {
        private static string InputGesturesToString(InputGestureCollection inputGestureCollection)
        {
            var builder = new StringBuilder();
            foreach (InputGesture g in inputGestureCollection)
            {
                var gesture = g as KeyGesture;
                if (gesture == null)
                {
                    // Ignore the mouse gestures
                    continue;
                }

                if (builder.Length != 0)
                {
                    builder.Append(", ");
                }

                builder.Append(gesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture));
            }

            return builder.ToString();
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
            var command = value as RoutedUICommand;
            return (command != null) 
                ? string.Format(
                    CultureInfo.CurrentCulture, 
                    "{0} ({1})", 
                    command.Text, 
                    InputGesturesToString(command.InputGestures)) 
                : string.Empty;
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
