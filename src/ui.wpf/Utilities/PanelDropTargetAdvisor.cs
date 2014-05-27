//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Apollo.UI.Wpf.Utilities
{
    /// <summary>
    /// A drag target advisor for a panel.
    /// </summary>
    /// <remarks>
    /// Original source here: http://blogs.msdn.com/b/llobo/archive/2006/12/08/drag-drop-library.aspx.
    /// </remarks>
    public sealed class PanelDropTargetAdvisor : IDropTargetAdvisor
    {
        private static readonly DataFormat s_SupportedFormat = DragDropHelpers.DataFormat;

        /// <summary>
        /// Gets or sets the UI element that is the target of the drag operation.
        /// </summary>
        public UIElement TargetUI
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a value indicating if the data object is valid for a drag
        /// operation that ends on the related UI element.
        /// </summary>
        /// <param name="obj">The data object.</param>
        /// <returns>
        /// <see langword="true"/> if the data object is valid for a drag operation that ends on the related UI element;
        /// otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="obj"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsValidDataObject(IDataObject obj)
        {
            {
                Lokad.Enforce.Argument(() => obj);
            }

            var storedObject = obj.GetData(s_SupportedFormat.Name);
            return DragDropManager.IsDragAllowed(TargetUI, storedObject);
        }

        /// <summary>
        /// Handles the clean-up after finishing the drag operation.
        /// </summary>
        /// <param name="obj">The data object that describes the data that was dropped.</param>
        /// <param name="dropPoint">The location on the control where the data was dropped.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="obj"/> is <see langword="null" />.
        /// </exception>
        public void OnDropCompleted(IDataObject obj, Point dropPoint)
        {
            {
                Lokad.Enforce.Argument(() => obj);
            }

            var storedObject = obj.GetData(s_SupportedFormat.Name);
            DragDropManager.ProcessDroppedObject(TargetUI, storedObject);
        }

        /// <summary>
        /// Creates the UI element that is used as the drag feedback item.
        /// </summary>
        /// <param name="obj">The data object that describes the data that is being dragged.</param>
        /// <returns>An UI element that is used as the drag object.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="obj"/> is <see langword="null" />.
        /// </exception>
        public UIElement GetVisualFeedback(IDataObject obj)
        {
            {
                Lokad.Enforce.Argument(() => obj);
            }

            var storedObject = obj.GetData(s_SupportedFormat.Name);
            return DragDropManager.GetDragVisualizationFor(TargetUI, storedObject);
        }

        /// <summary>
        /// Determines the offset of the drag.
        /// </summary>
        /// <param name="obj">The data object that describes the data that is being dragged.</param>
        /// <returns>The point indicating the offset.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="obj"/> is <see langword="null" />.
        /// </exception>
        public Point GetOffsetPoint(IDataObject obj)
        {
            {
                Lokad.Enforce.Argument(() => obj);
            }

            var p = (Point)obj.GetData(DragDropHelpers.OffsetPointDataFormatName);
            return p;
        }
    }
}
