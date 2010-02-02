﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Practices.Composite.Logging;

namespace Apollo.UI.Common.Bootstrapper
{
    /// <summary>
    /// Defines extension methods for <see cref="IContainerAdapter"/> objects.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    public static class ContainerAdapterExtensions
    {
        /// <summary>
        /// Registers a type in the container as a Singleton (one and only one instance in the container).
        /// </summary>
        /// <typeparam name="TFrom">The registration type.</typeparam>
        /// <typeparam name="TTo">The type implementing the registration type.</typeparam>
        /// <param name="adapter">The extended adapter.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered value.
        /// </returns>
        /// <source>
        /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
        /// </source>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Registration of types is based on both the input and output types.")]
        public static IContainerAdapter RegisterSingletonType<TFrom, TTo>(this IContainerAdapter adapter) where TTo : TFrom
        {
            return adapter.RegisterType<TFrom, TTo>(ContainerRegistrationScope.Singleton);
        }

        /// <summary>
        /// Registers a type in the container as an instance (new instance each time).
        /// </summary>
        /// <typeparam name="TFrom">The registration type.</typeparam>
        /// <typeparam name="TTo">The type implementing the registration type.</typeparam>
        /// <param name="adapter">The extended adapter.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered value.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Registration of types is based on both the input and output types.")]
        public static IContainerAdapter RegisterInstanceType<TFrom, TTo>(this IContainerAdapter adapter) where TTo : TFrom
        {
            return adapter.RegisterType<TFrom, TTo>(ContainerRegistrationScope.Instance);
        }

        /// <summary>
        /// Registers a type in the container as an instance (new instance each time).
        /// Convenience overload; prefer explicit <see cref="RegisterInstanceType{F,T}"/>.
        /// </summary>
        /// <typeparam name="TFrom">The registration type.</typeparam>
        /// <typeparam name="TTo">The type implementing the registration type.</typeparam>
        /// <param name="adapter">The extended adapter.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered value.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Registration of types is based on both the input and output types.")]
        public static IContainerAdapter RegisterType<TFrom, TTo>(this IContainerAdapter adapter) where TTo : TFrom
        {
            return adapter.RegisterInstanceType<TFrom, TTo>();
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <param name="adapter">The extended adapter.</param>
        /// <param name="fromType">The registration type.</param>
        /// <param name="toType">The type implementing the registration type.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered value.
        /// </returns>
        public static IContainerAdapter RegisterSingletonTypeIfMissing(
            this IContainerAdapter adapter,
            Type fromType, 
            Type toType)
        {
            return adapter.RegisterTypeIfMissing(fromType, toType, ContainerRegistrationScope.Singleton);
        }

        /// <summary>
        /// Registers a type in the container only if that type was not already registered.
        /// </summary>
        /// <typeparam name="TFrom">The registration type.</typeparam>
        /// <typeparam name="TTo">The type implementing the registration type.</typeparam>
        /// <param name="adapter">The extended adapter.</param>
        /// <returns>
        /// The <see cref="IContainerAdapter"/> with the newly registered value.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Registration of types is based on both the input and output types.")]
        public static IContainerAdapter RegisterSingletonTypeIfMissing<TFrom, TTo>(this IContainerAdapter adapter)
            where TTo : TFrom
        {
            return adapter.RegisterTypeIfMissing<TFrom, TTo>(ContainerRegistrationScope.Singleton);
        }

        #region IContainerAdapter Implementation Helper Methods to keep them DRY. Cheating by putting here.

        /// <summary>
        /// Log attempt to re-register type. Internal - for <see cref="IContainerAdapter"/> implementors.
        /// </summary>
        /// <typeparam name="T">The type of the service that must be reregistered.</typeparam>
        /// <param name="logger">The logger.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Use of a generic parameter provides strongly typed methods.")]
        public static void TypeMappingAlreadyRegistered<T>(ILoggerFacade logger)
        {
            TypeMappingAlreadyRegistered(logger, typeof(T));
        }

        /// <summary>
        /// Log attempt to re-register type. Internal - for <see cref="IContainerAdapter"/> implementors.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="type">The type that was reregistered.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "The registration is linked to the actual Type.")]
        public static void TypeMappingAlreadyRegistered(ILoggerFacade logger, Type type)
        {
            logger.Log(
                String.Format(
                    CultureInfo.CurrentCulture,
                    "Type mapping already registered {0}",
                    type.Name), 
                Category.Debug, 
                Priority.Low);
        }

        #endregion
    }
}