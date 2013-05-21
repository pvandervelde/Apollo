//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Autofac.Core;

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects that form a proxy for the dependency injection system.
    /// </summary>
    public interface IDependencyInjectionProxy
    {
        /// <summary>
        /// Resolves an instance of type T, with the given parameters.
        /// </summary>
        /// <typeparam name="T">The type of the object that must be resolved.</typeparam>
        /// <param name="parameters">The parameters for the type resolution.</param>
        /// <returns>The resolved instance.</returns>
        T Resolve<T>(params Parameter[] parameters);

        /// <summary>
        /// Resolves an instance of the provided type, with the given parameters.
        /// </summary>
        /// <param name="objectType">The type of object that must be resolved.</param>
        /// <param name="parameters">The parameters for the type resolution.</param>
        /// <returns>The resolved instance.</returns>
        object Resolve(Type objectType, params Parameter[] parameters);
    }
}
