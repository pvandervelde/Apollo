//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xml;

namespace Apollo.UI.Wpf.Utilities
{
    /// <summary>
    /// A drag target advisor for a panel.
    /// </summary>
    /// <remarks>
    /// Original source here: http://blogs.msdn.com/b/llobo/archive/2006/12/08/drag-drop-library.aspx
    /// </remarks>
    public sealed class PanelDropTargetAdvisor : IDropTargetAdvisor
    {
        private static DataFormat s_SupportedFormat = DragDropHelpers.DataFormat;

        private UIElement m_TargetUI;

        /// <summary>
        /// Gets or sets the UI element that is the target of the drag operation.
        /// </summary>
        public UIElement TargetUI
        {
            get
            {
                return m_TargetUI;
            }
            
            set
            {
                m_TargetUI = value;
            }
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
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsValidDataObject(IDataObject obj)
        {
            var storedObject = obj.GetData(s_SupportedFormat.Name);
            return DragDropManager.IsDragAllowed(TargetUI, storedObject);
        }

        /// <summary>
        /// Handles the clean-up after finishing the drag operation.
        /// </summary>
        /// <param name="obj">The data object that describes the data that was dropped.</param>
        /// <param name="dropPoint">The location on the control where the data was dropped.</param>
        public void OnDropCompleted(IDataObject obj, Point dropPoint)
        {
            var storedObject = obj.GetData(s_SupportedFormat.Name);
            DragDropManager.ProcessDroppedObject(TargetUI, storedObject);
        }

        /// <summary>
        /// Creates the UI element that is used as the drag feedback item.
        /// </summary>
        /// <param name="obj">The data object that describes the data that is being dragged.</param>
        /// <returns>An UI element that is used as the drag object.</returns>
        public UIElement GetVisualFeedback(IDataObject obj)
        {
            var storedObject = obj.GetData(s_SupportedFormat.Name);
            return DragDropManager.GetDragVisualizationFor(TargetUI, storedObject);
        }

        /// <summary>
        /// Determines the offset of the drag.
        /// </summary>
        /// <param name="obj">The data object that describes the data that is being dragged.</param>
        /// <returns>The point indicating the offset.</returns>
        public Point GetOffsetPoint(IDataObject obj)
        {
            Point p = (Point)obj.GetData(DragDropHelpers.OffsetPointDataFormatName);
            return p;
        }
    }
}
