//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Apollo.Core.Base;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// Converts between <see cref="DatasetCreator"/> and an opacity value.
    /// </summary>
    internal sealed class DatasetCreatorToOpacityConverter : IValueConverter
    {
        /// <summary>
        /// The collection that maps the dataset creator to a value indicating how important
        /// that creator. This value ranges between 0 (unimportant) and 1 (very important).
        /// </summary>
        /// <remarks>
        /// The values in this collection are used to set the opacity of the vertex on the
        /// canvas. System created datasets are considered less important and thus more
        /// see-through.
        /// </remarks>
        private static readonly Dictionary<DatasetCreator, double> s_CreatorToImportanceMap
            = new Dictionary<DatasetCreator, double>()
                {
                    { DatasetCreator.System, 0.65 },
                    { DatasetCreator.User, 1.0 },
                };

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
            if (targetType != typeof(double))
            {
                throw new InvalidOperationException();
            }

            if (!(value is DatasetCreator))
            {
                throw new InvalidOperationException();
            }

            var creator = (DatasetCreator)value;
            return s_CreatorToImportanceMap[creator];
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
