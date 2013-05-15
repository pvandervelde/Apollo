//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;
using Microsoft.Practices.ServiceLocation;

namespace Apollo.UI.Wpf.Bootstrappers
{
    /// <summary>
    /// This is a version of the UnityBootstrapper that is generic for any IOC container.
    /// </summary>
    /// <remarks>
    /// This class must be overridden to provide application specific configuration.
    /// <para>
    /// This is a cut-down version of the UnityBootstrapper from Prism.
    /// The Unity dependencies have been removed
    /// The WPF/SL dependencies remain but could be removed
    /// </para>
    /// </remarks>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    public abstract class CompositeBootstrapper
    {
        /// <summary>
        /// The catalog that holds all the modules.
        /// </summary>
        private readonly IModuleCatalog m_ModuleCatalog;

        /// <summary>
        /// Indicates if the default Prism services should be registered.
        /// </summary>
        private bool m_UseDefaultConfiguration = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeBootstrapper"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="moduleCatalog">The module catalog.</param>
        protected CompositeBootstrapper(IContainerAdapter container, IModuleCatalog moduleCatalog)
        {
            m_ModuleCatalog = moduleCatalog;
            Container = container;
        }

        /// <summary>
        /// Runs the bootstrapper process.
        /// </summary>
        public void Run()
        {
            Run(true);
        }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        /// <param name="useDefaultConfiguration">
        /// If <see langword="true"/>, registers default Composite Application Library services in the container. 
        /// This is the default behavior.
        /// </param>
        public void Run(bool useDefaultConfiguration)
        {
            // Run the bootstrapping process
            m_UseDefaultConfiguration = useDefaultConfiguration;
            {
                CreateIocContainer();
                ConfigureIocContainer();
                ConfigureRegionAdapters();

                RegisterFrameworkExceptionTypes();

                CreateAndConfigureShell();
                InitializeModules();
            }

            LogRunActivity("Bootstrapper sequence completed");
        }

        /// <summary>
        /// Gets the default <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        protected abstract ILoggerFacade LoggerFacade
        {
            get;
        }

        /// <summary>
        /// Log activity within the <see cref="Run(bool)"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks>
        /// Override with do-nothing if you don't want this chatter.
        /// </remarks>
        protected abstract void LogRunActivity(string message);

        /// <summary>
        /// Gets the default <see cref="IContainerAdapter"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IContainerAdapter"/> instance.</value>
        public IContainerAdapter Container 
        { 
            get;
            private set;
        }

        /// <summary>
        /// Creates the IOC container.
        /// </summary>
        private void CreateIocContainer()
        {
            LogRunActivity("Creating IoC container");

            if (Container == null)
            {
                throw new InvalidOperationException("Container should not be null.");
            }
        }

        /// <summary>
        /// Configures the IOC container.
        /// </summary>
        private void ConfigureIocContainer()
        {
            LogRunActivity("Configuring container");
            ConfigureContainer();
        }

        /// <summary>
        /// Configures the IoC container. 
        /// May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            RegisterStandardModules();
            if (m_UseDefaultConfiguration)
            {
                RegisterDefaultContainerConfiguration();
            }
        }

        /// <summary>
        /// Registers the standard modules.
        /// </summary>
        protected void RegisterStandardModules()
        {
            RegisterInstance(LoggerFacade);
            RegisterModuleCatalog();
        }

        /// <summary>
        /// Registers the default container configuration.
        /// </summary>
        private void RegisterDefaultContainerConfiguration()
        {
            RegisterSingletonType<IModuleInitializer, ModuleInitializer>();
            RegisterSingletonType<IModuleManager, ModuleManager>();
            RegisterSingletonType<RegionAdapterMappings, RegionAdapterMappings>();
            RegisterSingletonType<IRegionManager, RegionManager>();
            RegisterSingletonType<IEventAggregator, EventAggregator>();
            RegisterSingletonType<IRegionViewRegistry, RegionViewRegistry>();
            RegisterSingletonType<IRegionBehaviorFactory, RegionBehaviorFactory>();

            ServiceLocator.SetLocatorProvider(() => Container.Resolve<IServiceLocator>());
        }

        /// <summary>
        /// Registers the type as a singleton.
        /// </summary>
        /// <typeparam name="TOriginal">The original type.</typeparam>
        /// <typeparam name="TResolveAs">The type as which the objects should be resolved.</typeparam>
        private void RegisterSingletonType<TOriginal, TResolveAs>()
        {
            Container.RegisterType(typeof(TOriginal), typeof(TResolveAs), ContainerRegistrationScope.Singleton);
        }

        /// <summary>
        /// Registers an instance of type T in the container as a singleton.
        /// </summary>
        /// <typeparam name="T">The type of the instance which is being registered.</typeparam>
        /// <param name="instance">The instance being registered.</param>
        /// <remarks>
        /// This method is provided to make it easier to register within the container.
        /// </remarks>
        protected void RegisterInstance<T>(T instance) where T : class
        {
            Container.RegisterInstance(instance);
        }

        /// <summary>
        /// Configures the region adapters.
        /// </summary>
        private void ConfigureRegionAdapters()
        {
            LogRunActivity("Configuring region adapters");
            ConfigureRegionAdapterMappings();
            ConfigureDefaultRegionBehaviors();
        }

        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// May be overwritten in a derived class to add specific mappings required by the application.
        /// </summary>
        /// <returns>The <see cref="RegionAdapterMappings"/> instance containing all the mappings.</returns>
        protected virtual RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mapper = new RegionAdapterMapper(Container)
                .Map<Selector, SelectorRegionAdapter>()
                .Map<ItemsControl, ItemsControlRegionAdapter>()
                .Map<ContentControl, ContentControlRegionAdapter>();

            return mapper.Mappings;
        }

        /// <summary>
        /// Configures the <see cref="IRegionBehaviorFactory"/>. This will be the list of default
        /// behaviors that will be added to a region.
        /// </summary>
        /// <returns>
        /// The <see cref="IRegionBehaviorFactory"/> that holds the newly registered behaviors.
        /// </returns>
        protected virtual IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var mapper = new RegionBehaviorFactoryMapper(Container);

            mapper
                .Map<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey)
                .Map<BindRegionContextToDependencyObjectBehavior>(BindRegionContextToDependencyObjectBehavior.BehaviorKey)
                .Map<RegionActiveAwareBehavior>(RegionActiveAwareBehavior.BehaviorKey)
                .Map<SyncRegionContextWithHostBehavior>(SyncRegionContextWithHostBehavior.BehaviorKey)
                .Map<RegionManagerRegistrationBehavior>(RegionManagerRegistrationBehavior.BehaviorKey);

            return mapper.Factory;
        }

        /// <summary>
        /// Registers the <see cref="Type"/> of the Exceptions
        /// that are not considered root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected virtual void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ActivationException));
        }

        /// <summary>
        /// Creates the shell and configures it.
        /// </summary>
        protected abstract void CreateAndConfigureShell();

        /// <summary>
        /// Registers the default module catalog.
        /// </summary>
        private void RegisterModuleCatalog()
        {
            var catalog = m_ModuleCatalog;
            if (catalog != null)
            {
                Container.RegisterInstance(catalog);
            }
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog.
        /// </summary>
        protected virtual void InitializeModules()
        {
            LogRunActivity("Initializing modules");

            // The module manager should always be there. If the default
            // registration has taken place then it will be. If custom registrations
            // have taken place then it should have been ...
            var manager = Container.Resolve<IModuleManager>();
            manager.Run();
        }
    }
}
