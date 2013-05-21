//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Autofac;
using Autofac.Core;

namespace Apollo.Utilities
{
    /// <summary>
    /// Provides a proxy for the dependency injection framework.
    /// </summary>
    public sealed class DependencyInjectionProxy : IDependencyInjectionProxy
    {
        /// <summary>
        /// The actual IOC container.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionProxy"/> class.
        /// </summary>
        /// <param name="container">The IOC container.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="container"/> is <see langword="null" />.
        /// </exception>
        public DependencyInjectionProxy(IContainer container)
        {
            {
                Lokad.Enforce.Argument(() => container);
            }

            m_Container = container;
        }

        /// <summary>
        /// Resolves an instance of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object that must be resolved.</typeparam>
        /// <param name="parameters">The parameters for the type resolution.</param>
        /// <returns>The resolved instance.</returns>
        public T Resolve<T>(params Parameter[] parameters)
        {
            return m_Container.Resolve<T>(parameters);
        }

        /// <summary>
        /// Resolves an instance of the provided type, with the given parameters.
        /// </summary>
        /// <param name="objectType">The type of object that must be resolved.</param>
        /// <param name="parameters">The parameters for the type resolution.</param>
        /// <returns>The resolved instance.</returns>
        public object Resolve(Type objectType, params Parameter[] parameters)
        {
            return m_Container.Resolve(objectType, parameters);
        }
    }
}
