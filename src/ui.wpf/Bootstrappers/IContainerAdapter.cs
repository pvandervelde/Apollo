//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Wpf.Bootstrappers
{
    /// <summary>
    /// Interface for adapter facilitating simple type registration and 
    /// service location with the IoC container.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    public interface IContainerAdapter
    {
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
        IContainerAdapter RegisterInstance<T>(T instance) where T : class;

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
        IContainerAdapter RegisterType<TFrom, TTo>(ContainerRegistrationScope scope) where TTo : TFrom;

        /// <summary>
        /// Registers a type in the container.
        /// </summary>
        /// <param name="fromType">The registration type.</param>
        /// <param name="toType">The type implementing the registration type.</param>
        /// <param name="scope">Scope of the registered type.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered element.
        /// </returns>
        IContainerAdapter RegisterType(Type fromType, Type toType, ContainerRegistrationScope scope);

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
        IContainerAdapter RegisterTypeIfMissing<TFrom, TTo>(ContainerRegistrationScope scope) where TTo : TFrom;

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <param name="fromType">The registration type.</param>
        /// <param name="toType">The type implementing the registration type.</param>
        /// <param name="scope">Scope of the registered type.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered element.
        /// </returns>
        IContainerAdapter RegisterTypeIfMissing(Type fromType, Type toType, ContainerRegistrationScope scope);

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
        T Resolve<T>() where T : class;

        /// <summary>
        /// Get an instance of object of type <see param="type"/>.
        /// </summary>
        /// <param name="type">The type of the requested service.</param>
        /// <returns>The requested service.</returns>
        /// <remarks>See <see cref="Resolve{T}"/></remarks>
        object Resolve(Type type);

        /// <summary>
        /// Try to get an instance of object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the requested service.</typeparam>
        /// <returns>
        /// The sought object or null if not found or can't create.
        /// </returns>
        /// <remarks>See <see cref="Resolve{T}"/>.</remarks>
        T TryResolve<T>() where T : class;

        /// <summary>
        /// Try to get an instance of object of type <see param="type"/>.
        /// </summary>
        /// <param name="type">The type of the requested service.</param>
        /// <returns>
        /// The sought object or null if not found or can't create.
        /// </returns>
        /// <remarks>See <see cref="TryResolve{T}"/></remarks>
        object TryResolve(Type type);
    }
}
