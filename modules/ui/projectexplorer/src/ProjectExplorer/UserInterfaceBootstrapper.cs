//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.UI.Common.Bootstrapper;
using Autofac;
using Lokad;
using Microsoft.Practices.Composite.Logging;
using Microsoft.Practices.Composite.Modularity;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// The bootstrapper for the User Interface.
    /// </summary>
    internal sealed class UserInterfaceBootstrapper : CompositeBootstrapper
    {
        /// <summary>
        /// The IOC container.
        /// </summary>
        private readonly IContainer m_IocContainer;

        /// <summary>
        /// The default logger. To be replaced by the proper one.
        /// </summary>
        private readonly ILoggerFacade m_Logger = new TextLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterfaceBootstrapper"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="container"/> is <see langword="null" />.
        /// </exception>
        public UserInterfaceBootstrapper(IContainer container)
            : base(new AutofacContainerAdapter(container), new ModuleCatalog().AddModule(typeof(ProjectExplorerModule)))
        {
            {
                Enforce.Argument(() => container);
            }

            m_IocContainer = container;
        }

        #region Overrides of CompositeBootstrapper

        /// <summary>
        /// Gets the default <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        protected override ILoggerFacade LoggerFacade
        {
            get
            {
                return m_Logger;
            }
        }

        /// <summary>
        /// Log activity within the <see cref="CompositeBootstrapper.Run(bool)"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>
        /// Override with do-nothing if you don't want this chatter.
        /// </remarks>
        protected override void LogRunActivity(string message)
        {
            // Just ignore everything ...
        }

        /// <summary>
        /// Creates the shell and configures it.
        /// </summary>
        protected override void CreateAndConfigureShell()
        {
            m_IocContainer.Configure(builder => builder.RegisterType<ProjectExplorerModule>());
        }

        #endregion
    }
}
