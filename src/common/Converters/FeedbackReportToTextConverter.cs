//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Apollo.UI.Common.Properties;
using Apollo.UI.Common.Views.Feedback;

namespace Apollo.UI.Common.Converters
{
    /// <summary>
    /// Converts an <see cref="FeedbackFileModel"/> object to a string representation.
    /// </summary>
    public sealed class FeedbackReportToTextConverter : IValueConverter
    {
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
            var model = value as FeedbackFileModel;
            if (model == null)
            {
                throw new ArgumentException(Resources.Exceptions_Messages_ArgumentException, "value");
            }

            var application = Path.GetFileNameWithoutExtension(model.Path);
            application = application.Substring(0, application.IndexOf('_'));
            var result = string.Format(
                CultureInfo.CurrentUICulture,
                Resources.FeedbackToTextConverter_FeedbackModel,
                application,
                model.Date);

            return result;
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
