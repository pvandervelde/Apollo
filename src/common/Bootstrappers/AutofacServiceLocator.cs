//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Autofac;
using Microsoft.Practices.ServiceLocation;

namespace Apollo.UI.Common.Bootstrappers
{
    /// <summary>
    /// An AutoFac based <see cref="IServiceLocator"/>.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Autofac",
        Justification = "The correct spelling is 'Autofac'.")]
    public class AutofacServiceLocator : IServiceLocator
    {
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Autofac",
            Justification = "Autofac is the correct spelling for the Autofac IOC library.")]
        private readonly IContainer m_AutofacContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceLocator"/> class.
        /// </summary>
        /// <param name="autofacContainer">The autofac container.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "autofac",
            Justification = "The correct spelling is 'Autofac'.")]
        public AutofacServiceLocator(IContainer autofacContainer)
        {
            m_AutofacContainer = autofacContainer;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return m_AutofacContainer.Resolve(serviceType);
        }

        /// <summary>
        /// Get an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <returns>The requested service instance.</returns>
        /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is an error resolving
        /// the service instance.</exception>
        public object GetInstance(Type serviceType)
        {
            return m_AutofacContainer.Resolve(serviceType);
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="key">Name the object was registered with.</param>
        /// <returns>The requested service instance.</returns>
        /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is an error resolving
        /// the service instance.</exception>
        public object GetInstance(Type serviceType, string key)
        {
            return m_AutofacContainer.ResolveNamed(key, serviceType);
        }

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <returns>
        /// A sequence of instances of the requested <paramref name="serviceType"/>.
        /// </returns>
        /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            var enumerable = (IEnumerable)m_AutofacContainer.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType));
            return enumerable.Cast<object>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>The requested service.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Use of a generic parameter provides strongly typed methods.")]
        public TService GetInstance<TService>()
        {
            return m_AutofacContainer.Resolve<TService>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="key">The key under which the service is registered.</param>
        /// <returns>The requested service.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Use of a generic parameter provides strongly typed methods.")]
        public TService GetInstance<TService>(string key)
        {
            return m_AutofacContainer.ResolveNamed<TService>(key);
        }

        /// <summary>
        /// Gets all instances.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>A collection of services.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Use of a generic parameter provides strongly typed methods.")]
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            var enumerable = (IEnumerable)m_AutofacContainer.Resolve<IEnumerable<TService>>();
            return enumerable.OfType<TService>();
        }
    }
}
