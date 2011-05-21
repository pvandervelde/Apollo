//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac;
using Lokad;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;
using Microsoft.Practices.ServiceLocation;

namespace Apollo.UI.Common.Bootstrappers
{
    /// <summary>
    /// An <see cref="IContainerAdapter"/> for an Autofac <see cref="IContainer"/>.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Autofac",
        Justification = "The correct spelling is 'Autofac'.")]
    public class AutofacContainerAdapter : IContainerAdapter
    {
        /// <summary>
        /// Gets the main PRISM assembly.
        /// </summary>
        /// <returns>The requested assembly.</returns>
        private static Assembly MainPrismAssembly()
        {
            // Get by way of this known type
            return typeof(AutoPopulateRegionBehavior).Assembly;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacContainerAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public AutofacContainerAdapter(IContainer container) : this(container, new TextLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacContainerAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="loggerFacade">The logger facade.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="loggerFacade"/> is <see langword="null" />.
        /// </exception>
        public AutofacContainerAdapter(IContainer container, ILoggerFacade loggerFacade)
        {
            {
                Enforce.Argument(() => loggerFacade);
            }

            Container = container;
            Logger = loggerFacade;

            RegisterKnownConcreteClasses();

            RegisterInstance(Container)
                .RegisterInstance<IServiceLocator>(new AutofacServiceLocator(Container))
                .RegisterInstance<IContainerAdapter>(this); // optional .. do it if using IUnityContainer in your app.
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>The current instance.</returns>
        protected AutofacContainerAdapter Initialize()
        {
            var builder = new ContainerBuilder();

            // N.B.: Build can only be called once. This is it!
            Container = builder.Build();
            return this;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private ILoggerFacade Logger 
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        private IContainer Container 
        { 
            get;
            set;
        }

        #region IContainerAdapter

        /// <summary>
        /// Registers an instance of type T in the container as a Singleton.
        /// </summary>
        /// <typeparam name="T">The type of the instance which will be its registered type.</typeparam>
        /// <param name="instance">Object to register.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered element.
        /// </returns>
        /// <Remarks>
        /// Not meaningful to register a non-Singleton instance as there is no way to retrieve it.
        /// </Remarks>
        public IContainerAdapter RegisterInstance<T>(T instance) where T : class
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterInstance(instance);
            }

            builder.Update(Container);
            return this;
        }

        /// <summary>
        /// Registers a type in the container.
        /// </summary>
        /// <typeparam name="TFrom">The registration type.</typeparam>
        /// <typeparam name="TTo">The type implementing the registration type.</typeparam>
        /// <param name="scope">How to register the type.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered element.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Registration of types is based on both the input and output types.")]
        public IContainerAdapter RegisterType<TFrom, TTo>(ContainerRegistrationScope scope) where TTo : TFrom
        {
            var builder = new ContainerBuilder();
            {
                if (scope.IsSingleton())
                {
                    builder.RegisterType<TTo>().As<TFrom>().SingleInstance();
                }
                else
                {
                    builder.RegisterType<TTo>().As<TFrom>().InstancePerDependency();
                }
            }

            builder.Update(Container);
            return this;
        }

        /// <summary>
        /// Registers a type in the container.
        /// </summary>
        /// <param name="fromType">The registration type.</param>
        /// <param name="toType">The type implementing the registration type.</param>
        /// <param name="scope">Scope of the registered type.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered element.
        /// </returns>
        public IContainerAdapter RegisterType(Type fromType, Type toType, ContainerRegistrationScope scope)
        {
            var builder = new ContainerBuilder();
            {
                if (scope.IsSingleton())
                {
                    builder.RegisterType(toType).As(fromType).SingleInstance();
                }
                else
                {
                    builder.RegisterType(toType).As(fromType).InstancePerDependency();
                }
            }

            builder.Update(Container);
            return this;
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <typeparam name="TFrom">The registration type.</typeparam>
        /// <typeparam name="TTo">The type implementing the registration type.</typeparam>
        /// <param name="scope">Scope of the registered type.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered element.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Registration of types is based on both the input and output types.")]
        public IContainerAdapter RegisterTypeIfMissing<TFrom, TTo>(ContainerRegistrationScope scope) where TTo : TFrom
        {
            if (Container.IsRegistered<TFrom>())
            {
                Logger.Log(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        "Type is already registered: {0}", 
                        typeof(TFrom).Name), 
                        Category.Warn, 
                        Priority.Medium);
                ContainerAdapterExtensions.TypeMappingAlreadyRegistered<TFrom>(Logger);
            }
            else
            {
                RegisterType<TFrom, TTo>(scope);
            }

            return this;
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <param name="fromType">The registration type.</param>
        /// <param name="toType">The type implementing the registration type.</param>
        /// <param name="scope">Scope of the registered type.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered element.
        /// </returns>
        public IContainerAdapter RegisterTypeIfMissing(Type fromType, Type toType, ContainerRegistrationScope scope)
        {
            if (Container.IsRegistered(fromType))
            {
                ContainerAdapterExtensions.TypeMappingAlreadyRegistered(Logger, fromType);
            }
            else
            {
                RegisterType(fromType, toType, scope);
            }

            return this;
        }

        /// <summary>
        /// Get an instance of object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the requested service.</typeparam>
        /// <returns>The requested service.</returns>
        /// <remarks>
        /// Caller doesn't know, and presumably need not know, if the
        /// instance is new or a singleton.
        /// <para> Throw exception if can't return instance.</para>
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Use of a generic parameter provides strongly typed methods.")]
        public T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// Get an instance of object of type <see param="type"/>.
        /// </summary>
        /// <param name="type">The type of the requested service.</param>
        /// <returns>The requested service.</returns>
        /// <remarks>See <see cref="Resolve{T}"/></remarks>
        public object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        /// <summary>
        /// Try to get an instance of object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the requested service.</typeparam>
        /// <returns>
        /// The sought object or null if not found or can't create.
        /// </returns>
        /// <remarks>See <see cref="Resolve{T}"/>.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Use of a generic parameter provides strongly typed methods.")]
        public T TryResolve<T>() where T : class
        {
            object result;
            Container.TryResolve(typeof(T), out result);
            return (T)result;
        }

        /// <summary>
        /// Try to get an instance of object of type <see param="type"/>.
        /// </summary>
        /// <param name="type">The type of the requested service.</param>
        /// <returns>
        /// The sought object or null if not found or can't create.
        /// </returns>
        /// <remarks>See <see cref="TryResolve{T}"/></remarks>
        public object TryResolve(Type type)
        {
            object result;
            Container.TryResolve(type, out result);
            return result;
        }

        #endregion

        #region RegisterKnownConcreteClasses

        /// <summary>
        /// Explictily register concrete classes resolved by Prism components.
        /// </summary>
        /// <remarks>
        /// These concrete type registrations were not in the original bootstrapper
        /// You do not need to register them with Unity or StructureMap
        /// because these containers resolve unregistered concrete types
        /// with Instance (Factory) scope.
        /// <para>
        /// But some IoC containers don't resolve unregistered concrete classes.
        /// Autofac is one such container.
        /// </para><para>
        /// I just winged it here, through combination of experiment and a
        /// peek at Nick's attempt at Prism integration, described here:
        /// http://code.google.com/p/autofac/wiki/PrismIntegration
        /// </para>
        /// </remarks>
        private void RegisterKnownConcreteClasses()
        {
            var prismTypes = MainPrismAssembly().GetTypes();

            var desiredTypes =
                from type in prismTypes
                where
                    type.IsClass && 
                    !type.IsAbstract &&
                    (typeof(IRegionAdapter).IsAssignableFrom(type) || typeof(IRegionBehavior).IsAssignableFrom(type))
                select type;

            var builder = new ContainerBuilder();
            {
                foreach (var each in desiredTypes)
                {
                    var localObj = each;
                    builder.RegisterType(localObj)
                        .InstancePerDependency()
                        .ExternallyOwned();
                }

                // Other known types missed above
                builder.RegisterType<DelayedRegionCreationBehavior>()
                        .InstancePerDependency()
                        .ExternallyOwned();
            }

            builder.Update(Container);
        }

        #endregion
    }
}
