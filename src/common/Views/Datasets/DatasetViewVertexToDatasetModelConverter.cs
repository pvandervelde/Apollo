//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// Converts from a <see cref="DatasetViewVertex"/> to a <see cref="DatasetModel"/>.
    /// </summary>
    /// <design>
    /// This converter is required because the <c>VertexControl</c> style for the
    /// <see cref="DatasetGraphView"/> has a <c>DatasetViewVertex</c> but the 
    /// <see cref="DatasetVertexView"/> (note the word order) needs a <c>DatasetModel</c>.
    /// Unfortunately we can't call methods in XAML so we'll use a converter.
    /// </design>
    internal sealed class DatasetViewVertexToDatasetModelConverter : IValueConverter
    {
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
            // The target type will be a System.Object because we're assigning to the DataContext
            // of a UIElement.
            var vertex = value as DatasetViewVertex;
            if (vertex == null)
            {
                throw new InvalidOperationException();
            }

            return vertex.Model;
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
