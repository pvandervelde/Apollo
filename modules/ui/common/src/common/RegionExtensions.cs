//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Composite.Regions;

namespace Apollo.UI.Common
{
    /// <summary>
    /// A set of extensions to provide presenter and parameter support for Composite WPF's region manager.
    /// </summary>
    public static class RegionExtensions
    {
        /// <summary>
        /// A dependency property set on each view to associate it with the parameter that created it.
        /// </summary>
        public static readonly DependencyProperty ParameterProperty = 
            DependencyProperty.RegisterAttached("Parameter", typeof(Parameter), typeof(RegionExtensions), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the parameter associated with a view.
        /// </summary>
        /// <param name="view">The view for which the parameter is requested.</param>
        /// <returns>
        /// The requested parameter.
        /// </returns>
        public static Parameter GetParameter(object view)
        {
            var target = view as DependencyObject;
            if (target == null)
            {
                return null;
            }

            return (Parameter)target.GetValue(ParameterProperty);
        }

        /// <summary>
        /// Sets the parameter associated with a view.
        /// </summary>
        /// <param name="view">The view for which a parameter must be registered.</param>
        /// <param name="parameter">The parameter which must be registered.</param>
        public static void SetParameter(object view, Parameter parameter)
        {
            var target = view as DependencyObject;
            if (target == null)
            {
                return;
            }

            target.SetValue(ParameterProperty, parameter);
        }

        /// <summary>
        /// Adds a view to a region and activates it.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="view">The view for the region.</param>
        public static void AddAndActivate(this IRegion region, object view)
        {
            region.Add(view);
            region.Activate(view);
        }

        /// <summary>
        /// Adds a view with a specified name to a region and activates it.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="view">The view for the region.</param>
        /// <param name="name">The name for the association.</param>
        public static void AddAndActivate(this IRegion region, object view, string name)
        {
            region.Add(view, name);
            region.Activate(view);
        }

        /// <summary>
        /// Adds a view with a specified name to a region and activates it.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="view">The view for the region.</param>
        /// <param name="createChildRegion">If set to <see langword="true" /> a child region manager will be created.</param>
        public static void AddAndActivate(this IRegion region, object view, bool createChildRegion)
        {
            region.Add(view, Guid.NewGuid().ToString(), createChildRegion);
            region.Activate(view);
        }

        /// <summary>
        /// Finds the view associated with the given parameter.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The view that is associated with the given parameter.</returns>
        public static object GetViewByParameter(this IRegion region, Parameter parameter)
        {
            return region.Views.OfType<DependencyObject>().FirstOrDefault(x => parameter.Equals(GetParameter(x)));
        }

        /// <summary>
        /// Activates the view associated with the given parameter.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="parameter">The parameter.</param>
        public static void ActivateByParameter(this IRegion region, Parameter parameter)
        {
            var view = region.GetViewByParameter(parameter);
            if (view != null)
            {
                region.Activate(view);
            }
        }

        /// <summary>
        /// Determines whether the region already has a view by the given parameter.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///     <see langword="true" /> if a view by this parameter already exists in the region; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool HasViewByParameter(this IRegion region, Parameter parameter)
        {
            return region.GetViewByParameter(parameter) != null;
        }

        /// <summary>
        /// Adds the given view and associates it with the given parameter.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="view">The view for the parameter.</param>
        /// <param name="parameter">The parameter.</param>
        public static void AddWithParameter(this IRegion region, object view, Parameter parameter)
        {
            SetParameter(view, parameter);
            region.Add(view);
        }

        /// <summary>
        /// Adds the given view and associates it with the given parameter.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="view">The view for the parameter.</param>
        /// <param name="parameter">The parameter.</param>
        public static void AddAndActivateWithParameter(this IRegion region, object view, Parameter parameter)
        {
            region.AddWithParameter(view, parameter);
            region.Activate(view);
        }
    }
}