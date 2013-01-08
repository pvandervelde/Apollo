//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Apollo.UI.Wpf.Utilities
{
    /// <summary>
    /// A drag source advisor for a panel.
    /// </summary>
    /// <remarks>
    /// Original source here: http://blogs.msdn.com/b/llobo/archive/2006/12/08/drag-drop-library.aspx
    /// </remarks>
    public sealed class PanelDragSourceAdvisor : IDragSourceAdvisor
    {
        private static DataFormat s_SupportedFormat = DragDropHelpers.DataFormat;
        private UIElement m_SourceUI;

        /// <summary>
        /// Gets or sets the source element where the drag started.
        /// </summary>
        public UIElement SourceUI
        {
            get
            {
                return m_SourceUI;
            }
            
            set
            {
                m_SourceUI = value;
            }
        }

        /// <summary>
        /// Gets the supported drag effects.
        /// </summary>
        public DragDropEffects SupportedEffects
        {
            get
            {
                return DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// Gets the data object for the drag operation.
        /// </summary>
        /// <param name="draggedElement">The object from which the drag started.</param>
        /// <param name="offsetPoint">The drag offset.</param>
        /// <returns>The data object for the drag operation.</returns>
        public DataObject GetDataObject(UIElement draggedElement, Point offsetPoint)
        {
            var obj = GetDataFromUIContainer(draggedElement);
            var data = new DataObject();
            data.SetData(s_SupportedFormat.Name, obj);
            data.SetData(DragDropHelpers.OffsetPointDataFormatName, offsetPoint);

            return data;
        }

        private object GetDataFromUIContainer(UIElement draggedElement)
        {
            var source = SourceUI as Panel;
            Debug.Assert(source != null, "This drag source advisor should only be used on Panel objects.");

            FrameworkElement container = draggedElement as FrameworkElement;
            Debug.Assert(container != null, "We should have found the container element now.");

            return container.DataContext;
        }

        /// <summary>
        /// Handles the clean-up after finishing the drag operation.
        /// </summary>
        /// <param name="draggedElement">The element that was dragged.</param>
        /// <param name="finalEffects">The end effects of the drag.</param>
        public void FinishDrag(UIElement draggedElement, DragDropEffects finalEffects)
        {
            // Do nothing for now ...
        }

        /// <summary>
        /// Determines if an element can be dragged.
        /// </summary>
        /// <param name="elementToDrag">The element.</param>
        /// <returns>
        /// <see langword="true"/> if the element can be dragged; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanBeDragged(UIElement elementToDrag)
        {
            var obj = GetDataFromUIContainer(elementToDrag);
            return DragDropManager.IsDragAllowed(SourceUI, obj);
        }
    }
}
