//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Apollo.UI.Common.Utilities
{
    /// <summary>
    /// An <see cref="IDragSourceAdvisor"/> that works on <see cref="ItemsControl"/> objects.
    /// </summary>
    public sealed class ItemsControlDragSourceAdvisor : IDragSourceAdvisor
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
            var source = SourceUI as ItemsControl;
            Debug.Assert(source != null, "This drag source advisor should only be used on ItemsControl objects.");

            FrameworkElement container = source.ContainerFromElement(draggedElement) as FrameworkElement;
            Debug.Assert(container != null, "We should have found the container element now.");

            var obj = container.DataContext;
            return obj;
        }

        /// <summary>
        /// Handles the clean-up after finishing the drag operation.
        /// </summary>
        /// <param name="draggedElement">The element that was dragged.</param>
        /// <param name="finalEffects">The end effects of the drag.</param>
        public void FinishDrag(UIElement draggedElement, DragDropEffects finalEffects)
        {
            // For now do nothing
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
        public bool IsDraggable(UIElement elementToDrag)
        {
            var obj = GetDataFromUIContainer(elementToDrag);
            return DragDropManager.IsDragAllowed(SourceUI, obj);
        }
    }
}
