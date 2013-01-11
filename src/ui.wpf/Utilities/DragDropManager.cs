//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Apollo.UI.Wpf.Utilities
{
    /// <summary>
    /// The class that provides the dependency properties used to handle drag and drop operations.
    /// </summary>
    /// <remarks>
    /// Original source here: http://blogs.msdn.com/b/llobo/archive/2006/12/08/drag-drop-library.aspx
    /// </remarks>
    public static class DragDropManager
    {
        /// <summary>
        /// The collection that maps a UI element to the function that generates the drag visualization
        /// for that model.
        /// </summary>
        private static readonly Dictionary<UIElement, Func<object, UIElement>> s_DragVisualizationMap
            = new Dictionary<UIElement, Func<object, UIElement>>();

        /// <summary>
        /// The collection that maps a UIElement to a function that handles the actual drop action.
        /// </summary>
        /// <remarks>
        /// There may be a rather large memory leak here. We never clear out the objects so they'll live on forever ...
        /// </remarks>
        private static readonly Dictionary<UIElement, Action<object>> s_DropHandlers
            = new Dictionary<UIElement, Action<object>>();

        /// <summary>
        /// The collection that holds the functions that determine if a drag is allowed to finish on a given
        /// UI element.
        /// </summary>
        private static readonly Dictionary<UIElement, Func<object, bool>> s_IsDragAllowed
            = new Dictionary<UIElement, Func<object, bool>>();

        private static UIElement s_DraggedElement;
        private static bool s_IsMouseDown;
        private static Point s_DragStartPoint;
        private static Point s_OffsetPoint;
        private static DropPreviewAdorner s_OverlayElement;

        /// <summary>
        /// The dependency property that specifies what the drag source advisor is for the current control.
        /// </summary>
        public static readonly DependencyProperty DragSourceAdvisorProperty = DependencyProperty.RegisterAttached(
            "DragSourceAdvisor", 
            typeof(IDragSourceAdvisor), 
            typeof(DragDropManager),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDragSourceAdvisorChanged)));

        /// <summary>
        /// The dependency property that specifies the method that determines if the current control allows dragging at this moment.
        /// </summary>
        public static readonly DependencyProperty IsDragAllowedProperty = DependencyProperty.RegisterAttached(
            "IsDragAllowed", 
            typeof(Func<object, bool>), 
            typeof(DragDropManager),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsDragAllowedChanged)));

        /// <summary>
        /// The dependency property that specifies what control template should be used for the visualization of the item that is about to be dragged.
        /// </summary>
        public static readonly DependencyProperty DragVisualizationTemplateProperty = DependencyProperty.RegisterAttached(
            "DragVisualizationTemplate",
            typeof(Func<object, UIElement>),
            typeof(DragDropManager),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDragVisualizationTemplateChanged)));

        /// <summary>
        /// The dependency property that specifies what the drag target advisor is for the current control.
        /// </summary>
        public static readonly DependencyProperty DropTargetAdvisorProperty = DependencyProperty.RegisterAttached(
            "DropTargetAdvisor", 
            typeof(IDropTargetAdvisor), 
            typeof(DragDropManager),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDropTargetAdvisorChanged)));

        /// <summary>
        /// The dependency property that specifies which method should be used to handle the drop operation.
        /// </summary>
        public static readonly DependencyProperty DropHandlerProperty = DependencyProperty.RegisterAttached(
            "DropHandler",
            typeof(Action<object>),
            typeof(DragDropManager),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDropHandlerChanged)));

        /// <summary>
        /// Indicates that the DragSourceAdvisor attached property is set or is not set. This method should
        /// only be used by the WPF system.
        /// </summary>
        /// <param name="obj">The dependency object on which the property should be set.</param>
        /// <param name="isSet">Indicates if the property is set or not set.</param>
        public static void SetDragSourceAdvisor(DependencyObject obj, bool isSet)
        {
            obj.SetValue(DragSourceAdvisorProperty, isSet);
        }

        /// <summary>
        /// Indicates that the IsDragAllowed attached property is set or is not set. This method should
        /// only be used by the WPF system.
        /// </summary>
        /// <param name="obj">The dependency object on which the property should be set.</param>
        /// <param name="isSet">Indicates if the property is set or not set.</param>
        public static void SetIsDragAllowed(DependencyObject obj, bool isSet)
        {
            obj.SetValue(IsDragAllowedProperty, isSet);
        }

        /// <summary>
        /// Indicates that the DragVisualizationTemplate attached property is set or is not set. This method should
        /// only be used by the WPF system.
        /// </summary>
        /// <param name="obj">The dependency object on which the property should be set.</param>
        /// <param name="isSet">Indicates if the property is set or not set.</param>
        public static void SetDragVisualizationTemplate(DependencyObject obj, bool isSet)
        {
            obj.SetValue(DragVisualizationTemplateProperty, isSet);
        }

        /// <summary>
        /// Indicates that the DropTargetAdvisor attached property is set or is not set. This method should
        /// only be used by the WPF system.
        /// </summary>
        /// <param name="obj">The dependency object on which the property should be set.</param>
        /// <param name="isSet">Indicates if the property is set or not set.</param>
        public static void SetDropTargetAdvisor(DependencyObject obj, bool isSet)
        {
            obj.SetValue(DropTargetAdvisorProperty, isSet);
        }

        /// <summary>
        /// Indicates that the DropHandler attached property is set or is not set. This method should
        /// only be used by the WPF system.
        /// </summary>
        /// <param name="obj">The dependency object on which the property should be set.</param>
        /// <param name="isSet">Indicates if the property is set or not set.</param>
        public static void SetDropHandler(DependencyObject obj, bool isSet)
        {
            obj.SetValue(DropHandlerProperty, isSet);
        }

        private static IDragSourceAdvisor GetDragSourceAdvisor(DependencyObject obj)
        {
            return obj.GetValue(DragSourceAdvisorProperty) as IDragSourceAdvisor;
        }

        private static Func<object, bool> GetIsDragAllowed(DependencyObject obj)
        {
            return obj.GetValue(IsDragAllowedProperty) as Func<object, bool>;
        }

        private static Func<object, UIElement> GetDragVisualizationTemplate(DependencyObject obj)
        {
            return obj.GetValue(DragVisualizationTemplateProperty) as Func<object, UIElement>;
        }

        private static IDropTargetAdvisor GetDropTargetAdvisor(DependencyObject obj)
        {
            return obj.GetValue(DropTargetAdvisorProperty) as IDropTargetAdvisor;
        }

        private static Action<object> GetDropHandler(DependencyObject obj)
        {
            return obj.GetValue(DropHandlerProperty) as Action<object>;
        }

        private static UIElement GetTopContainer()
        {
            return Application.Current.MainWindow.Content as UIElement;
        }

        private static void OnDragSourceAdvisorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            UIElement sourceElement = sender as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                sourceElement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSourcePreviewMouseLeftButtonDown);
                sourceElement.PreviewMouseMove += new MouseEventHandler(DragSourcePreviewMouseMove);
                sourceElement.PreviewMouseUp += new MouseButtonEventHandler(DragSourcePreviewMouseUp);

                // Set the Drag source UI
                IDragSourceAdvisor advisor = args.NewValue as IDragSourceAdvisor;
                advisor.SourceUI = sourceElement;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                sourceElement.PreviewMouseLeftButtonDown -= DragSourcePreviewMouseLeftButtonDown;
                sourceElement.PreviewMouseMove -= DragSourcePreviewMouseMove;
                sourceElement.PreviewMouseUp -= DragSourcePreviewMouseUp;
            }
        }

        private static void DragSourcePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Make this the new drag source
            IDragSourceAdvisor advisor = GetDragSourceAdvisor(sender as DependencyObject);

            if (advisor.CanBeDragged(e.Source as UIElement) == false)
            {
                return;
            }

            s_DraggedElement = e.Source as UIElement;
            s_DragStartPoint = e.GetPosition(GetTopContainer());
            s_OffsetPoint = e.GetPosition(s_DraggedElement);
            s_IsMouseDown = true;
        }

        private static void DragSourcePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (s_IsMouseDown && IsDragGesture(e.GetPosition(GetTopContainer())))
            {
                DragStarted(sender as UIElement);
            }
        }

        private static bool IsDragGesture(Point point)
        {
            bool hGesture = Math.Abs(point.X - s_DragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance;
            bool vGesture = Math.Abs(point.Y - s_DragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance;

            return hGesture | vGesture;
        }

        private static void DragStarted(UIElement uiElement)
        {
            s_IsMouseDown = false;
            Mouse.Capture(uiElement);

            IDragSourceAdvisor advisor = GetDragSourceAdvisor(uiElement as DependencyObject);
            DataObject data = advisor.GetDataObject(s_DraggedElement, s_OffsetPoint);
            DragDropEffects supportedEffects = advisor.SupportedEffects;

            // Perform DragDrop
            DragDropEffects effects = System.Windows.DragDrop.DoDragDrop(s_DraggedElement, data, supportedEffects);
            advisor.FinishDrag(s_DraggedElement, effects);

            // Clean up
            RemovePreviewAdorner();
            Mouse.Capture(null);
            s_DraggedElement = null;
        }

        private static void RemovePreviewAdorner()
        {
            if (s_OverlayElement != null)
            {
                AdornerLayer.GetAdornerLayer(GetTopContainer()).Remove(s_OverlayElement);
                s_OverlayElement = null;
            }
        }

        private static void DragSourcePreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            s_IsMouseDown = false;
            Mouse.Capture(null);
        }

        private static void OnIsDragAllowedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            UIElement targetElement = sender as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                if (!s_IsDragAllowed.ContainsKey(targetElement))
                {
                    var func = args.NewValue as Func<object, bool>;
                    s_IsDragAllowed.Add(targetElement, func);
                }
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                s_IsDragAllowed.Remove(targetElement);
            }
        }

        private static void OnDragVisualizationTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            UIElement targetElement = sender as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                if (!s_DragVisualizationMap.ContainsKey(targetElement))
                {
                    var func = args.NewValue as Func<object, UIElement>;
                    s_DragVisualizationMap.Add(targetElement, func);
                }
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                s_DragVisualizationMap.Remove(targetElement);
            }
        }

        private static void OnDropTargetAdvisorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            UIElement targetElement = sender as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                targetElement.PreviewDragEnter += new DragEventHandler(DropTargetPreviewDragEnter);
                targetElement.PreviewDragOver += new DragEventHandler(DropTargetPreviewDragOver);
                targetElement.PreviewDragLeave += new DragEventHandler(DropTargetPreviewDragLeave);
                targetElement.PreviewDrop += new DragEventHandler(DropTargetPreviewDrop);
                targetElement.AllowDrop = true;

                // Set the Drag source UI
                IDropTargetAdvisor advisor = args.NewValue as IDropTargetAdvisor;
                advisor.TargetUI = targetElement;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                targetElement.PreviewDragEnter -= DropTargetPreviewDragEnter;
                targetElement.PreviewDragOver -= DropTargetPreviewDragOver;
                targetElement.PreviewDragLeave -= DropTargetPreviewDragLeave;
                targetElement.PreviewDrop -= DropTargetPreviewDrop;
                targetElement.AllowDrop = false;
            }
        }

        private static void DropTargetPreviewDragEnter(object sender, DragEventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null)
            {
                return;
            }

            if (UpdateEffects(sender, e) == false)
            {
                return;
            }

            // Setup the preview Adorner
            UIElement feedbackUI = GetDropTargetAdvisor(uiElement).GetVisualFeedback(e.Data);
            s_OffsetPoint = GetDropTargetAdvisor(uiElement).GetOffsetPoint(e.Data);
            CreatePreviewAdorner(uiElement, feedbackUI);

            e.Handled = true;
        }

        private static bool UpdateEffects(object uiObject, DragEventArgs e)
        {
            IDropTargetAdvisor advisor = GetDropTargetAdvisor(uiObject as DependencyObject);
            if (advisor.IsValidDataObject(e.Data) == false)
            {
                return false;
            }

            if ((e.AllowedEffects & DragDropEffects.Move) == 0 &&
                (e.AllowedEffects & DragDropEffects.Copy) == 0)
            {
                e.Effects = DragDropEffects.None;
                return true;
            }

            if ((e.AllowedEffects & DragDropEffects.Move) != 0 &&
                (e.AllowedEffects & DragDropEffects.Copy) != 0)
            {
                if ((e.KeyStates & DragDropKeyStates.ControlKey) != 0)
                {
                }

                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
                    DragDropEffects.Copy : DragDropEffects.Move;
            }

            return true;
        }

        private static void CreatePreviewAdorner(UIElement adornedElement, UIElement feedbackUI)
        {
            // Clear if there is an existing preview adorner
            RemovePreviewAdorner();

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(GetTopContainer());
            s_OverlayElement = new DropPreviewAdorner(feedbackUI, adornedElement);
            layer.Add(s_OverlayElement);
        }

        private static void DropTargetPreviewDragOver(object sender, DragEventArgs e)
        {
            if (UpdateEffects(sender, e) == false)
            {
                return;
            }

            // Update position of the preview Adorner
            Point position = e.GetPosition(sender as UIElement);
            s_OverlayElement.Left = position.X - s_OffsetPoint.X;
            s_OverlayElement.Top = position.Y - s_OffsetPoint.Y;

            e.Handled = true;
        }

        private static void DropTargetPreviewDragLeave(object sender, DragEventArgs e)
        {
            if (UpdateEffects(sender, e) == false)
            {
                return;
            }

            RemovePreviewAdorner();
            e.Handled = true;
        }

        private static void DropTargetPreviewDrop(object sender, DragEventArgs e)
        {
            if (UpdateEffects(sender, e) == false)
            {
                return;
            }

            IDropTargetAdvisor advisor = GetDropTargetAdvisor(sender as DependencyObject);
            Point dropPoint = e.GetPosition(sender as UIElement);

            // Calculate displacement for (Left, Top)
            Point offset = e.GetPosition(s_OverlayElement);
            dropPoint.X = dropPoint.X - offset.X;
            dropPoint.Y = dropPoint.Y - offset.Y;

            advisor.OnDropCompleted(e.Data, dropPoint);
            RemovePreviewAdorner();
            s_OffsetPoint = new Point(0, 0);
        }

        private static void OnDropHandlerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            UIElement targetElement = sender as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                if (!s_DropHandlers.ContainsKey(targetElement))
                {
                    var func = args.NewValue as Action<object>;
                    s_DropHandlers.Add(targetElement, func);
                }
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                s_DropHandlers.Remove(targetElement);
            }
        }

        /// <summary>
        /// Gets the drag visualization element for the given model object.
        /// </summary>
        /// <param name="sender">The object that where the drag is ending.</param>
        /// <param name="model">The model object.</param>
        /// <returns>The drag visualization element.</returns>
        public static UIElement GetDragVisualizationFor(UIElement sender, object model)
        {
            UIElement visualization = null;
            if (s_DragVisualizationMap.ContainsKey(sender))
            {
                visualization = s_DragVisualizationMap[sender](model);
            }

            return visualization;
        }

        /// <summary>
        /// Processes the dropping of the object.
        /// </summary>
        /// <param name="dropTarget">The UI element on which the data is dropped.</param>
        /// <param name="model">The object that was dropped.</param>
        public static void ProcessDroppedObject(UIElement dropTarget, object model)
        {
            if (s_DropHandlers.ContainsKey(dropTarget))
            {
                s_DropHandlers[dropTarget](model);
            }
        }

        /// <summary>
        /// Returns a value indicating if the object is can be dragged from or onto the given UI element.
        /// </summary>
        /// <param name="source">The UI element.</param>
        /// <param name="model">The object that is being dragged.</param>
        /// <returns><see langword="true" /> if the object can be dragged; otherwise, <see langword="false" />.</returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsDragAllowed(UIElement source, object model)
        {
            bool result = false;
            if (s_IsDragAllowed.ContainsKey(source))
            {
                result = s_IsDragAllowed[source](model);
            }

            return result;
        }
    }
}
