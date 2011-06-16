//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Common;
using Autofac;

namespace Apollo.ProjectExplorer.Views.About
{
    /// <summary>
    /// The presenter for the <see cref="AboutModel"/>.
    /// </summary>
    internal sealed class AboutPresenter : Presenter<IAboutView, AboutModel, AboutParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields",
            Justification = "Currently not used but will be as soon as we actually implement this.")]
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the menu.</param>
        public AboutPresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            View.Model = new AboutModel(context);
        }
    }
}
