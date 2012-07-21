//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Apollo.UI.Common.Utilities
{
    /// <summary>
    /// Defines the interface for objects that advise on how to handle 
    /// a drag operation from the drag source side.
    /// </summary>
    /// <remarks>
    /// Original source here: http://blogs.msdn.com/b/llobo/archive/2006/12/08/drag-drop-library.aspx
    /// </remarks>
    public interface IDragSourceAdvisor
    {
        /// <summary>
        /// Gets or sets the source element where the drag started.
        /// </summary>
        UIElement SourceUI
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the supported drag effects.
        /// </summary>
        DragDropEffects SupportedEffects
        {
            get;
        }

        /// <summary>
        /// Gets the data object for the drag operation.
        /// </summary>
        /// <param name="draggedElement">The object from which the drag started.</param>
        /// <param name="offsetPoint">The drag offset.</param>
        /// <returns>The data object for the drag operation.</returns>
        DataObject GetDataObject(UIElement draggedElement, Point offsetPoint);

        /// <summary>
        /// Handles the clean-up after finishing the drag operation.
        /// </summary>
        /// <param name="draggedElement">The element that was dragged.</param>
        /// <param name="finalEffects">The end effects of the drag.</param>
        void FinishDrag(UIElement draggedElement, DragDropEffects finalEffects);

        /// <summary>
        /// Determines if an element can be dragged.
        /// </summary>
        /// <param name="elementToDrag">The element.</param>
        /// <returns>
        /// <see langword="true"/> if the element can be dragged; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanBeDragged(UIElement elementToDrag);
    }
}
