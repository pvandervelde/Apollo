//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Apollo.Core.Base;
using Apollo.Utilities;
using Autofac;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// Initializes the dependency injection system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class DependencyInjection
    {
        /// <summary>
        /// Creates the DI container.
        /// </summary>
        /// <param name="context">The application context that controls the life time of the application.</param>
        /// <returns>The DI container.</returns>
        public static IContainer Load(ApplicationContext context)
        {
            IContainer result = null;
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new UtilitiesModule());

                // Don't allow discovery on the dataset application because:
                // - The dataset application wouldn't know what to do with it anyway
                // - We don't want anybody talking to the application except for the
                //   application that started it.
                builder.RegisterModule(new BaseModule(false));
                builder.RegisterModule(new BaseModuleForDatasets(
                    () => CloseApplication(result),
                    file => LoadDatasetFile(result, file)));

                builder.Register(c => context)
                    .As<ApplicationContext>()
                    .ExternallyOwned();
            }

            result = builder.Build();
            return result;
        }

        private static void CloseApplication(IContainer container)
        {
            var context = container.Resolve<ApplicationContext>();
            
            container.Dispose();
            context.ExitThread();
        }

        private static void LoadDatasetFile(IContainer container, FileInfo fileToLoad)
        {
            // For now we fake this out by pretending it takes time to load.
            var progressAction = container.Resolve<IDatasetApplicationNotificationInvoker>();
            progressAction.RaiseOnProgress(0, new StreamLoadProgressMark());

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(33, new StreamLoadProgressMark());

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(66, new StreamLoadProgressMark());

            Thread.Sleep(1000);
            progressAction.RaiseOnProgress(100, new StreamLoadProgressMark());
        }
    }
}
