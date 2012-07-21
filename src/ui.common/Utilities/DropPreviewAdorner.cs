//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Apollo.UI.Common.Utilities
{
    /// <summary>
    /// A preview adorner that displays a view of the dragged data during the drag operation.
    /// </summary>
    /// <remarks>
    /// Original source here: http://blogs.msdn.com/b/llobo/archive/2006/12/08/drag-drop-library.aspx
    /// </remarks>
    public sealed class DropPreviewAdorner : Adorner
    {
        private ContentPresenter m_Presenter;
        private double m_Left = 0;
        private double m_Top = 0;

        /// <summary>
        /// Gets or sets the left position of the adorner.
        /// </summary>
        public double Left
        {
            get 
            { 
                return m_Left; 
            }

            set
            {
                m_Left = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Gets or sets the top position of the adorner.
        /// </summary>
        public double Top
        {
            get 
            { 
                return m_Top; 
            }

            set
            {
                m_Top = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropPreviewAdorner"/> class.
        /// </summary>
        /// <param name="feedbackUI">The UI element used as feedback element.</param>
        /// <param name="adornedElt">The element to bind the adorner to.</param>
        public DropPreviewAdorner(UIElement feedbackUI, UIElement adornedElt)
            : base(adornedElt)
        {
            m_Presenter = new ContentPresenter();
            m_Presenter.Content = feedbackUI;
            m_Presenter.IsHitTestVisible = false;
        }

        private void UpdatePosition()
        {
            AdornerLayer layer = this.Parent as AdornerLayer;
            if (layer != null)
            {
                layer.Update(AdornedElement);
            }
        }

        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>A System.Windows.Size object representing the amount of layout space needed by the adorner.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            m_Presenter.Measure(constraint);
            return m_Presenter.DesiredSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a System.Windows.FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_Presenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Overrides System.Windows.Media.Visual.GetVisualChild(System.Int32), and returns
        /// a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return m_Presenter;
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Returns a System.Windows.Media.Transform for the adorner, based on the transform that is currently applied to the adorned element.
        /// </summary>
        /// <param name="transform">The transform that is currently applied to the adorned element.</param>
        /// <returns>A transform to apply to the adorner.</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(new TranslateTransform(Left, Top));
            result.Children.Add(base.GetDesiredTransform(transform));

            return result;
        }
    }
}
