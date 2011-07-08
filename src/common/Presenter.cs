//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Common
{
    /// <summary>
    /// A base class for presenters.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes",
        Justification = "The presenter is associated with a view and a model. The parameter provides additional information.")]
    [ExcludeFromCodeCoverage]
    public abstract class Presenter<TView, TModel, TParameter> : IPresenter
        where TParameter : Parameter
        where TView : IView<TModel>
        where TModel : Model
    {
        /// <summary>
        /// Gets the parameter that was provided to this presenter. The parameter provides any information 
        /// the presenter should need in determining how to show or manage the view.
        /// </summary>
        /// <value>The parameter.</value>
        public TParameter Parameter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the view, the visual object provided to this presenter.
        /// </summary>
        /// <value>The view for the current presenter.</value>
        public TView View
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the event aggregator.
        /// </summary>
        /// <value>The event aggregator.</value>
        public IEventAggregator EventAggregator
        {
            get;
            set;
        }

        /// <summary>
        /// Called when the view is closed.
        /// </summary>
        protected virtual void OnViewClosed()
        {
        }

        /// <summary>
        /// Called when the view is closing.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewClosing(CancelEventArgs e)
        {
        }

        /// <summary>
        /// Called when the view is activated.
        /// </summary>
        protected virtual void OnViewActivated()
        {
        }

        /// <summary>
        /// Called when the view is deactivated.
        /// </summary>
        protected virtual void OnViewDeactivated()
        {
        }

        /// <summary>
        /// Called when the view is shown.
        /// </summary>
        protected virtual void OnViewShown()
        {
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        #region IPresenter Members

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
            Justification = "These methods should not be overriden in child classes.")]
        Type IPresenter.ViewType
        {
            get
            {
                return typeof(TView);
            }
        }

        /// <summary>
        /// Initializes the presenter with the specified view and parameter.
        /// </summary>
        /// <param name="view">The view that is associated with the current presenter.</param>
        /// <param name="parameter">The parameter that is associated with the current presenter.</param>
        void IPresenter.Initialize(object view, Parameter parameter)
        {
            View = (TView)view;
            Parameter = (TParameter)parameter;

            var standardView = View as IStandardView;
            if (standardView != null)
            {
                standardView.OnClosed += (x, y) => OnViewClosed();
                standardView.OnClosing += (x, y) => OnViewClosing(y);
                standardView.OnShown += (x, y) => OnViewShown();
            }

            var regionView = View as IRegionView;
            if (regionView != null)
            {
                regionView.OnActivated += (x, y) => OnViewActivated();
                regionView.OnDeactivated += (x, y) => OnViewDeactivated();
            }

            Initialize();
        }

        /// <summary>
        /// Gets the view that is associated with the current presenter.
        /// </summary>
        /// <value>The view that is associated with the current presenter.</value>
        object IPresenter.View
        {
            get
            {
                return View;
            }
        }

        /// <summary>
        /// Gets the parameter that is associated with the current presenter.
        /// </summary>
        /// <value>The parameter that is associated with the current presenter.</value>
        Parameter IPresenter.Parameter
        {
            get
            {
                return Parameter;
            }
        }

        #endregion
    }
}
