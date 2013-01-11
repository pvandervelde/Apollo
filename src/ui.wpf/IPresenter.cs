//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.UI.Wpf
{
    /// <summary>
    /// Explicitly implemented by presenters to connect the presenter to the view and parameter.
    /// </summary>
    public interface IPresenter
    {
        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        Type ViewType 
        { 
            get; 
        }

        /// <summary>
        /// Gets the view that is associated with the current presenter.
        /// </summary>
        /// <value>The view that is associated with the current presenter.</value>
        object View 
        { 
            get; 
        }

        /// <summary>
        /// Gets the parameter that is associated with the current presenter.
        /// </summary>
        /// <value>The parameter that is associated with the current presenter.</value>
        Parameter Parameter 
        { 
            get; 
        }

        /// <summary>
        /// Initializes the presenter with the specified view and parameter.
        /// </summary>
        /// <param name="view">The view that is associated with the current presenter.</param>
        /// <param name="parameter">The parameter that is associated with the current presenter.</param>
        void Initialize(object view, Parameter parameter);
    }
}
