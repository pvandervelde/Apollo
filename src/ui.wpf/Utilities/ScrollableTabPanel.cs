//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Apollo.UI.Wpf.Utilities
{
    /// <summary>
    /// A scrollable TabPanel control.
    /// </summary>
    /// <remarks>
    /// Taken from: http://www.blogs.intuidev.com/post/2010/02/15/WPF-TabControl-series-Part-4-Closeable-TabItems.aspx
    /// </remarks>
    public class ScrollableTabPanel : Panel, IScrollInfo, INotifyPropertyChanged
    {
        // The following GradientStopCollections are being used for assigning an OpacityMask
        // to child-controls that are only partially visible.
        private static GradientStopCollection s_OpacityMaskStopsTransparentOnLeftAndRight = new GradientStopCollection
            {
                new GradientStop(Colors.Transparent, 0.0),
                new GradientStop(Colors.Black, 0.2),
                new GradientStop(Colors.Black, 0.8),
                new GradientStop(Colors.Transparent, 1.0)
            };

        private static GradientStopCollection s_OpacityMaskStopsTransparentOnLeft = new GradientStopCollection
            {
                new GradientStop(Colors.Transparent, 0),
                new GradientStop(Colors.Black, 0.5)
            };

        private static GradientStopCollection s_OpacityMaskStopsTransparentOnRight = new GradientStopCollection
            {
                new GradientStop(Colors.Black, 0.5),
                new GradientStop(Colors.Transparent, 1)
            };

        /// <summary>
        /// Calculates the HorizontalOffset for a given child-control, based on a desired value.
        /// </summary>
        /// <param name="viewportLeft">The left offset of the Viewport.</param>
        /// <param name="viewportRight">The right offset of the Viewport.</param>
        /// <param name="childLeft">The left offset of the control in question.</param>
        /// <param name="childRight">The right offset of the control in question.</param>
        /// <returns>The new scroll offset.</returns>
        private static double CalculateNewScrollOffset(
              double viewportLeft,
              double viewportRight,
              double childLeft,
              double childRight)
        {
            // Retrieve basic information about the position of the Viewport within the Extent of the control.
            bool isFurtherToLeft = (childLeft < viewportLeft) && (childRight < viewportRight);
            bool isFurtherToRight = (childRight > viewportRight) && (childLeft > viewportLeft);
            bool isWiderThanViewport = (childRight - childLeft) > (viewportRight - viewportLeft);

            if (!isFurtherToRight && !isFurtherToLeft)
            {
                // Don't change anything - the Viewport is completely visible (inside the Extent's bounds)
                return viewportLeft;
            }

            if (isFurtherToLeft && !isWiderThanViewport)
            {
                // The child is to be placed with its left edge equal to the left edge of the Viewport's present offset.
                return childLeft;
            }

            // The child is to be placed with its right edge equal to the right edge of the Viewport's present offset.
            return childRight - (viewportRight - viewportLeft);
        }

        /// <summary>
        /// Will remove the OpacityMask for all child controls.
        /// </summary>
        /// <param name="child">The child.</param>
        private static void RemoveOpacityMask(UIElement child)
        {
            child.OpacityMask = null;
        }

        // For a description of the members below, refer to the respective property's description.
        private ScrollViewer m_OwningScrollViewer;
        private bool m_CanScrollHorizontally = true;
        private Size m_ControlExtent = new Size(0, 0);
        private Size m_Viewport = new Size(0, 0);
        private Vector m_Offset;

        /// <summary>
        /// This will apply the present scroll-position resp. -offset.
        /// </summary>
        private TranslateTransform m_ScrollTransform = new TranslateTransform();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollableTabPanel"/> class.
        /// </summary>
        public ScrollableTabPanel()
        {
            this.RenderTransform = m_ScrollTransform;
            this.SizeChanged += new SizeChangedEventHandler(ScrollableTabPanelSizeChanged);
        }

        /// <summary>
        /// Compares the present sizes (Extent/Viewport) against the local values
        /// and updates them, if required.
        /// </summary>
        /// <param name="extent">The extent of the view.</param>
        /// <param name="viewportSize">The size of the viewport.</param>
        private void UpdateMembers(Size extent, Size viewportSize)
        {
            if (extent != this.Extent)
            {
                // The Extent of the control has changed.
                this.Extent = extent;
                if (this.ScrollOwner != null)
                {
                    this.ScrollOwner.InvalidateScrollInfo();
                }
            }

            if (viewportSize != this.Viewport)
            {
                // The Viewport of the panel has changed.
                this.Viewport = viewportSize;
                if (this.ScrollOwner != null)
                {
                    this.ScrollOwner.InvalidateScrollInfo();
                }
            }

            // Prevent from getting off to the right
            if (this.HorizontalOffset + this.Viewport.Width + this.RightOverflowMargin > this.ExtentWidth)
            {
                SetHorizontalOffset(HorizontalOffset + this.Viewport.Width + this.RightOverflowMargin);
            }

            // Notify UI-subscribers
            NotifyPropertyChanged("CanScroll");
            NotifyPropertyChanged("CanScrollLeft");
            NotifyPropertyChanged("CanScrollRight");
        }

        /// <summary>
        /// Returns the left position of the requested child (in Viewport-coordinates).
        /// </summary>
        /// <param name="child">The child to retrieve the position for.</param>
        /// <returns>The left edge position.</returns>
        private double GetLeftEdge(UIElement child)
        {
            double width = 0;
            double widthTotal = 0;

            // Loop through all child controls, summing up their required width
            foreach (UIElement uie in this.InternalChildren)
            {
                // The width of the current child control
                width = uie.DesiredSize.Width;

                if (child != null && child == uie)
                {
                    // The current child control is the one in question, so disregard its width
                    // and return the total width required for all controls further to the left,
                    // equaling the left edge of the requested child control.
                    return widthTotal;
                }

                // Sum up the overall width while the child control in question hasn't been hit.
                widthTotal += width;
            }

            // This shouldn't really be hit as the requested control should've been found beforehand.
            return widthTotal;
        }

        /// <summary>
        /// Determines whether the passed child control is only partially visible
        /// (i.e. whether part of it is outside of the Viewport).
        /// </summary>
        /// <param name="child">The child control to be tested.</param>
        /// <returns>
        /// True if part of the control is further to the left or right of the
        /// Viewport, False otherwise.
        /// </returns>
        public bool IsPartlyVisible(UIElement child)
        {
            Rect rctIntersect = GetIntersectionRectangle(child);
            return !(rctIntersect == Rect.Empty);
        }

        /// <summary>
        /// Determines the visible part of the passed child control, 
        /// measured between 0 (completely invisible) and 1 (completely visible),
        /// that is overflowing into the right invisible portion of the panel.
        /// </summary>
        /// <param name="child">The child control to be tested.</param>
        /// <returns>
        /// <para>A number between 0 (the control is completely invisible resp. outside of
        /// the Viewport) and 1 (the control is completely visible).</para>
        /// <para>All values between 0 and 1 indicate the part that is visible
        /// (i.e. 0.4 would mean that 40% of the control is visible, the remaining
        /// 60% will overflow into the right invisible portion of the panel.</para>
        /// </returns>
        public double PartlyVisiblePortionOverflowToRight(UIElement child)
        {
            Rect rctIntersect = GetIntersectionRectangle(child);
            double dblVisiblePortion = 1;
            if (!(rctIntersect == Rect.Empty)
                && this.CanScrollRight
                && rctIntersect.Width < child.DesiredSize.Width
                && rctIntersect.X > 0)
            {
                dblVisiblePortion = rctIntersect.Width / child.DesiredSize.Width;
            }

            return dblVisiblePortion;
        }

        /// <summary>
        /// Determines the visible part of the passed child control, 
        /// measured between 0 (completely invisible) and 1 (completely visible),
        /// that is overflowing into the left invisible portion of the panel.
        /// </summary>
        /// <param name="child">The child control to be tested.</param>
        /// <returns>
        /// <para>A number between 0 (the control is completely invisible resp. outside of
        /// the Viewport) and 1 (the control is completely visible).</para>
        /// <para>All values between 0 and 1 indicate the part that is visible
        /// (i.e. 0.4 would mean that 40% of the control is visible, the remaining
        /// 60% will overflow into the left invisible portion of the panel.</para>
        /// </returns>
        public double PartlyVisiblePortionOverflowToLeft(UIElement child)
        {
            Rect rctIntersect = GetIntersectionRectangle(child);
            double dblVisiblePortion = 1;
            if (!(rctIntersect == Rect.Empty)
                && this.CanScrollLeft
                && rctIntersect.Width < child.DesiredSize.Width
                && rctIntersect.X == 0)
            {
                dblVisiblePortion = rctIntersect.Width / child.DesiredSize.Width;
            }

            return dblVisiblePortion;
        }

        /// <summary>
        /// Returns the currently rendered rectangle that makes up the Viewport.
        /// </summary>
        /// <returns>The rectangle.</returns>
        private Rect GetScrollViewerRectangle()
        {
            return new Rect(new Point(0, 0), this.ScrollOwner.RenderSize);
        }

        /// <summary>
        /// Returns the rectangle that defines the outer bounds of a child control.
        /// </summary>
        /// <param name="child">The child/control for which to return the bounding rectangle.</param>
        /// <returns>The child rectangle.</returns>
        private Rect GetChildRectangle(UIElement child)
        {
            // Retrieve the position of the requested child inside the ScrollViewer control
            GeneralTransform childTransform = child.TransformToAncestor(this.ScrollOwner);
            return childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
        }

        /// <summary>
        /// Returns a Rectangle that contains the intersection between the ScrollViewer's
        /// and the passed child control's boundaries, that is, the portion of the child control
        /// which is currently visibile within the ScrollViewer's Viewport.
        /// </summary>
        /// <param name="child">The child for which to retrieve Rectangle.</param>
        /// <returns>The child rectangle.</returns>
        private Rect GetIntersectionRectangle(UIElement child)
        {
            // Retrieve the ScrollViewer's rectangle
            Rect scrollViewerRectangle = GetScrollViewerRectangle();
            Rect childRect = GetChildRectangle(child);

            // Return the area/rectangle in which the requested child and the ScrollViewer control's Viewport intersect.
            return Rect.Intersect(scrollViewerRectangle, childRect);
        }

        /// <summary>
        /// Will remove the OpacityMask for all child controls.
        /// </summary>
        private void RemoveOpacityMasks()
        {
            foreach (UIElement uieChild in Children)
            {
                RemoveOpacityMask(uieChild);
            }
        }

        /// <summary>
        /// Will check all child controls and set their OpacityMasks.
        /// </summary>
        private void UpdateOpacityMasks()
        {
            foreach (UIElement uieChild in Children)
            {
                UpdateOpacityMask(uieChild);
            }
        }

        /// <summary>
        /// Takes the given child control and checks as to whether the control is completely
        /// visible (in the Viewport). If not (i.e. if it's only partially visible), an OpacityMask
        /// will be applied so that it fades out into nothingness.
        /// </summary>
        /// <param name="child">The child.</param>
        private void UpdateOpacityMask(UIElement child)
        {
            if (child == null)
            {
                return;
            }

            // Retrieve the ScrollViewer's rectangle
            Rect scrollViewerRectangle = GetScrollViewerRectangle();
            if (scrollViewerRectangle == Rect.Empty)
            {
                return;
            }

            // Retrieve the child control's rectangle
            Rect childRect = GetChildRectangle(child);

            if (scrollViewerRectangle.Contains(childRect))
            {
                // This child is completely visible, so dump the OpacityMask.
                child.OpacityMask = null;
            }
            else
            {
                double partlyVisiblePortionOverflowToLeft = PartlyVisiblePortionOverflowToLeft(child);
                double partlyVisiblePortionOverflowToRight = PartlyVisiblePortionOverflowToRight(child);

                if (partlyVisiblePortionOverflowToLeft < 1 && partlyVisiblePortionOverflowToRight < 1)
                {
                    child.OpacityMask = new LinearGradientBrush(
                          s_OpacityMaskStopsTransparentOnLeftAndRight,
                          new Point(0, 0),
                          new Point(1, 0));
                }
                else if (partlyVisiblePortionOverflowToLeft < 1)
                {
                    // A part of the child (to the left) remains invisible, so fade out to the left.
                    child.OpacityMask = new LinearGradientBrush(
                          s_OpacityMaskStopsTransparentOnLeft,
                          new Point(1 - partlyVisiblePortionOverflowToLeft, 0),
                          new Point(1, 0));
                }
                else if (partlyVisiblePortionOverflowToRight < 1)
                {
                    // A part of the child (to the right) remains invisible, so fade out to the right.
                    child.OpacityMask = new LinearGradientBrush(
                          s_OpacityMaskStopsTransparentOnRight,
                          new Point(0, 0),
                          new Point(partlyVisiblePortionOverflowToRight, 0));
                }
                else
                {
                    // This child is completely visible, so dump the OpacityMask.
                    // Actually, this part should never be reached as, in this case, the very first
                    // checkup should've resulted in the child-rect being completely contained in
                    // the SV's rect; Well, I'll leave this here anyhow (just to be save).
                    child.OpacityMask = null;
                }
            }
        }

        /// <summary>
        /// This is the 1st pass of the layout process. Here, the Extent's size is being determined.
        /// </summary>
        /// <param name="availableSize">The Viewport's rectangle, as obtained after the 1st pass (MeasureOverride).</param>
        /// <returns>The Viewport's final size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // The default size will not reflect any width (i.e., no children) and always the default height.
            Size resultSize = new Size(0, availableSize.Height);

            // Loop through all child controls ...
            foreach (UIElement uieChild in this.InternalChildren)
            {
                // retrieve the desired size of the control ...
                uieChild.Measure(availableSize);

                // and pass this on to the size we need for the Extent
                resultSize.Width += uieChild.DesiredSize.Width;
            }

            UpdateMembers(resultSize, availableSize);

            double dblNewWidth = double.IsPositiveInfinity(availableSize.Width)
                ? resultSize.Width
                : availableSize.Width;

            resultSize.Width = dblNewWidth;
            return resultSize;
        }

        /// <summary>
        /// This is the 2nd pass of the layout process, where child controls are
        /// being arranged within the panel.
        /// </summary>
        /// <param name="finalSize">The Viewport's rectangle, as obtained after the 1st pass (MeasureOverride).</param>
        /// <returns>The Viewport's final size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.InternalChildren == null || this.InternalChildren.Count < 1)
            {
                return finalSize;
            }

            double dblWidth = 0;
            double dblWidthTotal = 0;
            foreach (UIElement uieChild in this.InternalChildren)
            {
                dblWidth = uieChild.DesiredSize.Width;
                uieChild.Arrange(new Rect(dblWidthTotal, 0, dblWidth, uieChild.DesiredSize.Height));
                dblWidthTotal += dblWidth;
            }

            return finalSize;
        }

        /// <summary>
        /// Invoked when the System.Windows.Media.VisualCollection of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The System.Windows.Media.Visual that was added to the collection.</param>
        /// <param name="visualRemoved">The System.Windows.Media.Visual that was removed from the collection.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            UpdateOpacityMasks();
        }

        /// <summary>
        /// Supports layout behavior when a child element is resized.
        /// </summary>
        /// <param name="child">The child element that is being resized.</param>
        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            UpdateOpacityMasks();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is allowed to scroll horizontally.
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get
            {
                return m_CanScrollHorizontally;
            }

            set
            {
                m_CanScrollHorizontally = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is allowed to scroll vertically.
        /// </summary>
        /// <remarks>
        /// This is DISABLED for the control! Due to the internal plumbing of the ScrollViewer
        /// control, this property needs to be accessible without an exception being thrown;
        /// however, setting this property will do plain nothing.
        /// </remarks>
        public bool CanVerticallyScroll
        {
            // We'll never be able to vertically scroll.
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets the height of the control; since no vertical scrolling has been
        /// implemented, this will return the same value at all times.
        /// </summary>
        public double ExtentHeight
        {
            get
            {
                return this.Extent.Height;
            }
        }

        /// <summary>
        /// Gets the overall width of the content hosted in the panel (i.e., the width
        /// measured between [far left of the scrollable portion] and [far right of the scrollable portion].
        /// </summary>
        public double ExtentWidth
        {
            get
            {
                return this.Extent.Width;
            }
        }

        /// <summary>
        /// Gets the current horizontal scroll offset.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return m_Offset.X;
            }

            private set
            {
                m_Offset.X = value;
            }
        }

        /// <summary>
        /// Increments the vertical offset.
        /// </summary>
        /// <remarks>This is unsupported.</remarks>
        public void LineDown()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Decrements the horizontal offset by the amount specified in the <see cref="LineScrollPixelCount"/> property.
        /// </summary>
        public void LineLeft()
        {
            SetHorizontalOffset(this.HorizontalOffset - this.LineScrollPixelCount);
        }

        /// <summary>
        /// Increments the horizontal offset by the amount specified in the <see cref="LineScrollPixelCount"/> property.
        /// </summary>
        public void LineRight()
        {
            SetHorizontalOffset(this.HorizontalOffset + this.LineScrollPixelCount);
        }

        /// <summary>
        /// Decrements the vertical offset.
        /// </summary>
        /// <remarks>This is unsupported.</remarks>
        public void LineUp()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Scrolls a child of the panel (Visual) into view.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>The new rectangle for the child.</returns>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (rectangle.IsEmpty || visual == null
              || visual == this || !IsAncestorOf(visual))
            {
                return Rect.Empty;
            }

            double dblOffsetX = 0;
            UIElement uieControlToMakeVisible = null;
            for (int i = 0; i < this.InternalChildren.Count; i++)
            {
                if ((Visual)this.InternalChildren[i] == visual)
                {
                    uieControlToMakeVisible = this.InternalChildren[i];
                    dblOffsetX = GetLeftEdge(this.InternalChildren[i]);
                    break;
                }
            }

            // Set the offset only if the desired element is not already completely visible.
            if (uieControlToMakeVisible != null)
            {
                if (uieControlToMakeVisible == this.InternalChildren[0])
                {
                    // If the first child has been selected, go to the very beginning of the scrollable area
                    dblOffsetX = 0;
                }
                else if (uieControlToMakeVisible == this.InternalChildren[this.InternalChildren.Count - 1])
                {
                    // If the last child has been selected, go to the very end of the scrollable area
                    dblOffsetX = this.ExtentWidth - this.Viewport.Width;
                }
                else
                {
                    dblOffsetX = CalculateNewScrollOffset(
                             this.HorizontalOffset,
                             this.HorizontalOffset + this.Viewport.Width,
                             dblOffsetX,
                             dblOffsetX + uieControlToMakeVisible.DesiredSize.Width);
                }

                SetHorizontalOffset(dblOffsetX);
                rectangle = new Rect(this.HorizontalOffset, 0, uieControlToMakeVisible.DesiredSize.Width, this.Viewport.Height);
            }

            return rectangle;
        }

        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelDown()
        {
            // We won't be responding to the mouse-wheel.
        }

        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelLeft()
        {
            // We won't be responding to the mouse-wheel.
        }

        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelRight()
        {
            // We won't be responding to the mouse-wheel.
        }

        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelUp()
        {
            // We won't be responding to the mouse-wheel.
        }

        /// <summary>
        /// Scrolls down within content by one page.
        /// </summary>
        public void PageDown()
        {
            // We won't be responding to vertical paging.
        }

        /// <summary>
        /// Scrolls left within content by one page.
        /// </summary>
        public void PageLeft()
        {
            // We won't be responding to horizontal paging.
        }

        /// <summary>
        /// Scrolls right within content by one page.
        /// </summary>
        public void PageRight()
        {
            // We won't be responding to horizontal paging.
        }

        /// <summary>
        /// Scrolls up within content by one page.
        /// </summary>
        public void PageUp()
        {
            // We won't be responding to vertical paging.
        }

        /// <summary>
        /// Gets or sets the ScrollViewer control that hosts the panel.
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get
            {
                return m_OwningScrollViewer;
            }

            set
            {
                m_OwningScrollViewer = value;
                if (m_OwningScrollViewer != null)
                {
                    this.ScrollOwner.Loaded += new RoutedEventHandler(ScrollOwnerLoaded);
                }
                else
                {
                    this.ScrollOwner.Loaded -= new RoutedEventHandler(ScrollOwnerLoaded);
                }
            }
        }

        /// <summary>
        /// Sets the horizontal offset for the tab header.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void SetHorizontalOffset(double offset)
        {
            // Remove all OpacityMasks while scrolling.
            RemoveOpacityMasks();

            // Assure that the horizontal offset always contains a valid value
            this.HorizontalOffset = Math.Max(0, Math.Min(this.ExtentWidth - this.Viewport.Width, Math.Max(0, offset)));

            if (this.ScrollOwner != null)
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }

            // If you don't want the animation, you would replace all the code further below (up to but not including)
            // the call to InvalidateMeasure() with the following line:
            // _ttScrollTransform.X = (-this.HorizontalOffset);
            //
            // Animate the new offset
            DoubleAnimation daScrollAnimation =
               new DoubleAnimation(
                     m_ScrollTransform.X,
                     (-this.HorizontalOffset),
                     new Duration(this.AnimationTimeSpan),
                     FillBehavior.HoldEnd);

            // Note that, depending on distance between the original and the target scroll-position and
            // the duration of the animation, the  acceleration and deceleration effects might be more
            // or less unnoticeable at runtime.
            daScrollAnimation.AccelerationRatio = 0.5;
            daScrollAnimation.DecelerationRatio = 0.5;

            // The childrens' OpacityMask can only be set reliably after the scroll-animation
            // has finished its work, so attach to the animation's Completed event where the
            // masks will be re-created.
            daScrollAnimation.Completed += new EventHandler(ScrollAnimationCompleted);

            m_ScrollTransform.BeginAnimation(
                  TranslateTransform.XProperty,
                  daScrollAnimation,
                  HandoffBehavior.Compose);

            InvalidateMeasure();
        }

        /// <summary>
        /// Sets the vertical offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void SetVerticalOffset(double offset)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the vertical offset.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the height of the viewport.
        /// </summary>
        public double ViewportHeight
        {
            get
            {
                return this.Viewport.Height;
            }
        }

        /// <summary>
        /// Gets the width of the viewport.
        /// </summary>
        public double ViewportWidth
        {
            get
            {
                return this.Viewport.Width;
            }
        }

        /// <summary>
        /// Gets the overall resp. internal/inner size of the control/panel.
        /// </summary>
        public Size Extent
        {
            get
            {
                return m_ControlExtent;
            }

            private set
            {
                m_ControlExtent = value;
            }
        }

        /// <summary>
        /// Gets the outer resp. visible size of the control/panel.
        /// </summary>
        public Size Viewport
        {
            get
            {
                return m_Viewport;
            }

            private set
            {
                m_Viewport = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the panel's scroll-position is on the far left (i.e. cannot scroll further to the left).
        /// </summary>
        public bool IsOnFarLeft
        {
            get
            {
                return this.HorizontalOffset == 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the panel's scroll-position is on the far right (i.e. cannot scroll further to the right).
        /// </summary>
        public bool IsOnFarRight
        {
            get
            {
                return (this.HorizontalOffset + this.Viewport.Width) == this.ExtentWidth;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the panel's viewport is larger than the control's extent, meaning there is hidden content 
        /// that the user would have to scroll for in order to see it.
        /// </summary>
        public bool CanScroll
        {
            get
            {
                return this.ExtentWidth > this.Viewport.Width;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the panel's scroll-position is NOT on the far left (i.e. can scroll to the left).
        /// </summary>
        public bool CanScrollLeft
        {
            get
            {
                return this.CanScroll && !this.IsOnFarLeft;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the panel's scroll-position is NOT on the far right (i.e. can scroll to the right).
        /// </summary>
        public bool CanScrollRight
        {
            get
            {
                return this.CanScroll && !this.IsOnFarRight;
            }
        }

        /// <summary>
        /// A dependency property that allows setting of the Margin that will be applied to the rightmost item in the panel.
        /// </summary>
        public static readonly DependencyProperty RightOverflowMarginProperty =
            DependencyProperty.Register(
                "RightOverflowMargin", 
                typeof(int), 
                typeof(ScrollableTabPanel),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets or retrieves the Margin that will be applied to the rightmost item in the panel;
        /// This allows for the item applying a negative margin, i.e. when selected.
        /// If set to a value other than zero (being the default), the control will add the value
        /// specified here to the item's right extent.
        /// </summary>
        public int RightOverflowMargin
        {
            get
            {
                return (int)GetValue(RightOverflowMarginProperty);
            }

            set
            {
                SetValue(RightOverflowMarginProperty, value);
            }
        }

        /// <summary>
        /// A dependency property that allows setting the duration (default: 100ms) for the panel's transition-animation that is
        /// started when an item is selected (scroll from the previously selected item to the
        /// presently selected one).
        /// </summary>
        public static readonly DependencyProperty AnimationTimeSpanProperty =
            DependencyProperty.Register(
                "AnimationTimeSpanProperty", 
                typeof(TimeSpan), 
                typeof(ScrollableTabPanel),
                new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0, 0, 100), FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the duration (default: 100ms) for the panel's transition-animation that is
        /// started when an item is selected (scroll from the previously selected item to the
        /// presently selected one).
        /// </summary>
        public TimeSpan AnimationTimeSpan
        {
            get
            {
                return (TimeSpan)GetValue(AnimationTimeSpanProperty);
            }

            set
            {
                SetValue(AnimationTimeSpanProperty, value);
            }
        }

        /// <summary>
        /// The amount of pixels to scroll by for the LineLeft() and LineRight() methods.
        /// </summary>
        public static readonly DependencyProperty LineScrollPixelCountProperty =
            DependencyProperty.Register(
                "LineScrollPixelCount", 
                typeof(int), 
                typeof(ScrollableTabPanel),
                new FrameworkPropertyMetadata(15, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the number of pixels to scroll by when the LineLeft or LineRight methods are called (default: 15px).
        /// </summary>
        public int LineScrollPixelCount
        {
            get
            {
                return (int)GetValue(LineScrollPixelCountProperty);
            }

            set
            {
                SetValue(LineScrollPixelCountProperty, value);
            }
        }

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP0100:AdvancedNamingRules",
            Justification = "Event is inherited from the INotifyProperyChanged interface.")]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called from within this class whenever subscribers (i.e. bindings) are to be notified of a property-change.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Fired when the ScrollViewer is initially loaded/displayed. 
        /// Required in order to initially setup the childrens' OpacityMasks.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void ScrollOwnerLoaded(object sender, RoutedEventArgs e)
        {
            UpdateOpacityMasks();
        }

        /// <summary>
        /// Fired when the scroll-animation has finished its work, that is, at the
        /// point in time when the ScrollViewerer has reached its final scroll-position
        /// resp. offset, which is when the childrens' OpacityMasks can be updated.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void ScrollAnimationCompleted(object sender, EventArgs e)
        {
            UpdateOpacityMasks();

            // This is required in order to update the TabItems' FocusVisual
            foreach (UIElement uieChild in this.InternalChildren)
            {
                uieChild.InvalidateArrange();
            }
        }

        private void ScrollableTabPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOpacityMasks();
        }
    }
}
