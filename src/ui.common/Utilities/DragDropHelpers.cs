//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;

namespace Apollo.UI.Common.Utilities
{
    /// <summary>
    /// Defines a set of helper methods and properties for drag and drop operations.
    /// </summary>
    public static class DragDropHelpers
    {
        /// <summary>
        /// Gets the string that is used to indicate the drag offset point in the drag data.
        /// </summary>
        public const string OffsetPointDataFormatName = "point";

        /// <summary>
        /// Gets the DataFormat used for drag and drop operations inside the apollo framework.
        /// </summary>
        public static DataFormat DataFormat
        {
            get
            {
                return DataFormats.GetDataFormat("Apollo.Internal.DragDropFormat");
            }
        }
    }
}
