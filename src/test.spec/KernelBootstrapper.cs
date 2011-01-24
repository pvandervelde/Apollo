//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core;
using Apollo.Utils;
using Apollo.Utils.ExceptionHandling;
using Autofac.Core;

namespace Test.Spec
{
    /// <summary>
    /// A boostrapper that starts the Apollo kernel.
    /// </summary>
    internal sealed class KernelBootstrapper : Bootstrapper
    {
        /// <summary>
        /// The function that allows storing a DI container.
        /// </summary>
        private readonly Action<IModule> m_ContainerStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBootstrapper"/> class.
        /// </summary>
        /// <param name="startInfo">The collection of <c>AppDomain</c> base and private paths.</param>
        /// <param name="exceptionHandlerFactory">The factory used for the creation of <see cref="IExceptionHandler"/> objects.</param>
        /// <param name="progress">The object used to track the progress of the bootstrapping process.</param>
        /// <param name="containerStorage">The function used to store the DI container which holds the kernel UI references.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="startInfo"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="exceptionHandlerFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="progress"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="containerStorage"/> is <see langword="null"/>.
        /// </exception>
        public KernelBootstrapper(
            KernelStartInfo startInfo,
            Func<IExceptionHandler> exceptionHandlerFactory,
            ITrackProgress progress,
            Action<IModule> containerStorage)
            : base(startInfo, exceptionHandlerFactory, progress)
        {
            m_ContainerStorage = containerStorage;
        }

        #region Overrides of Bootstrapper

        /// <summary>
        /// Stores the dependency injection container.
        /// </summary>
        /// <param name="container">The DI container.</param>
        protected override void StoreContainer(IModule container)
        {
            m_ContainerStorage(container);
        }

        #endregion
    }
}
