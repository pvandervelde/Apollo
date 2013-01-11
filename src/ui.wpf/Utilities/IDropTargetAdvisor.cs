//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Apollo.UI.Wpf.Utilities
{
    /// <summary>
    /// Defines the interface for objects that advise on how to handle 
    /// a drag operation from the drag target side.
    /// </summary>
    /// <remarks>
    /// Original source here: http://blogs.msdn.com/b/llobo/archive/2006/12/08/drag-drop-library.aspx
    /// </remarks>
    public interface IDropTargetAdvisor
    {
        /// <summary>
        /// Gets or sets the UI element that is the target of the drag operation.
        /// </summary>
        UIElement TargetUI
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
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsValidDataObject(IDataObject obj);
        
        /// <summary>
        /// Handles the clean-up after finishing the drag operation.
        /// </summary>
        /// <param name="obj">The data object that describes the data that was dropped.</param>
        /// <param name="dropPoint">The location on the control where the data was dropped.</param>
        void OnDropCompleted(IDataObject obj, Point dropPoint);
        
        /// <summary>
        /// Creates the UI element that is used as the drag feedback item.
        /// </summary>
        /// <param name="obj">The data object that describes the data that is being dragged.</param>
        /// <returns>An UI element that is used as the drag object.</returns>
        UIElement GetVisualFeedback(IDataObject obj);
        
        /// <summary>
        /// Determines the offset of the drag.
        /// </summary>
        /// <param name="obj">The data object that describes the data that is being dragged.</param>
        /// <returns>The point indicating the offset.</returns>
        Point GetOffsetPoint(IDataObject obj);
    }
}
