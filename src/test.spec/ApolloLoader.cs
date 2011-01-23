//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;
using Autofac.Core;

namespace Test.Spec
{
    /// <summary>
    /// Loads the apollo core.
    /// </summary>
    internal static class ApolloLoader
    {
        /// <summary>
        /// Loads the Apollo core.
        /// </summary>
        /// <param name="onKernelLoad">The method called when the User Interface section of the core is loaded.</param>
        public static void Load(Action<IModule> onKernelLoad)
        {
            // At a later stage we need to clean this up.
            // there are two constants and a DI reference.
            var progressTracker = new TimeBasedProgressTracker(
                new ProgressTimer(new TimeSpan(0, 0, 0, 0, 500)),
                -1,
                new StartupTimeStorage());

            Load(onKernelLoad, progressTracker);
        }

        /// <summary>
        /// Loads the Apollo core.
        /// </summary>
        /// <param name="onKernelLoad">The method called when the User Interface section of the core is loaded.</param>
        /// <param name="progressTracker">The object used for progress tracking.</param>
        public static void Load(Action<IModule> onKernelLoad, ITrackProgress progressTracker)
        {
            var bootstrapper = new KernelBootstrapper(
                new BootstrapperStartInfo(),
                () => new MockExceptionHandler(),
                progressTracker,
                onKernelLoad);

            // Load the kernel
            bootstrapper.Load();
        }
    }
}
