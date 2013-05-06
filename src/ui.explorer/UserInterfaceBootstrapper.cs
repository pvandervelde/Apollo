//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Apollo.UI.Wpf.Bootstrappers;
using Autofac;
using Lokad;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;

namespace Apollo.UI.Explorer
{
    /// <summary>
    /// The bootstrapper for the User Interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that this bootstrapper only takes care of the bootstrapping
    /// of the UI, not the core. By design the core and the UI are 
    /// running with different IOC containers / bootstrapper objects. This means
    /// that we can force a code separation because the UI controls cannot
    /// get linked to any of the internal core elements. The only way for
    /// the core and the UI to interact is via the UserInterfaceService.
    /// </para>
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "The logger doesn't die until the application is terminated. No point in having it be disposable.")]
    [ExcludeFromCodeCoverage]
    internal sealed class UserInterfaceBootstrapper : CompositeBootstrapper
    {
        /// <summary>
        /// The IOC container that contains the UI controls.
        /// </summary>
        private readonly IContainer m_ShellIocContainer;

        /// <summary>
        /// The reset event that is used to signal the application that it is safe to shut down.
        /// </summary>
        private readonly AutoResetEvent m_ResetEvent;

        /// <summary>
        /// The default logger. To be replaced by the proper one.
        /// </summary>
        private readonly ILoggerFacade m_Logger = new TextLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterfaceBootstrapper"/> class.
        /// </summary>
        /// <param name="container">The container provides the shell components.</param>
        /// <param name="resetEvent">The reset event used to indicate to the application that it is safe to shut down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="container"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="resetEvent"/> is <see langword="null" />.
        /// </exception>
        public UserInterfaceBootstrapper(IContainer container, AutoResetEvent resetEvent)
            : base(new AutofacContainerAdapter(container), new ModuleCatalog().AddModule(typeof(ExplorerModule)))
        {
            {
                Enforce.Argument(() => container);
                Enforce.Argument(() => resetEvent);
            }

            m_ShellIocContainer = container;
            m_ResetEvent = resetEvent;
        }

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
            var builder = new ContainerBuilder();
            {
                builder.Register(c => m_ResetEvent)
                    .As<AutoResetEvent>()
                    .ExternallyOwned();

                // Note that this 'module' is a Prism module, not an Autofac one!
                builder.RegisterType<ExplorerModule>();
            }
            
            builder.Update(m_ShellIocContainer);
        }
    }
}
