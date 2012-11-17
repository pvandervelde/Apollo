//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Scheduling;
using Apollo.Utilities;
using Autofac;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Defines methods to load an <see cref="IAssemblyScanner"/> object into a remote <c>AppDomain</c>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class AppDomainPluginClassLoader : MarshalByRefObject
    {
        /// <summary>
        /// Loads the <see cref="IAssemblyScanner"/> object into the <c>AppDomain</c> in which the current
        /// object is currently loaded.
        /// </summary>
        /// <param name="repository">The object that contains all the part and part group information.</param>
        /// <param name="logger">The object that provides the logging for the remote <c>AppDomain</c>.</param>
        /// <returns>The newly created <see cref="IAssemblyScanner"/> object.</returns>
        public IAssemblyScanner Load(IPluginRepository repository, ILogMessagesFromRemoteAppDomains logger)
        {
            try
            {
                var builder = new ContainerBuilder();
                {
                    builder.RegisterModule(new BaseModuleForScheduling());

                    builder.Register(c => new PartImportEngine(
                            c.Resolve<ISatisfyPluginRequests>()))
                        .As<IConnectParts>();
                    
                    builder.RegisterInstance(repository)
                        .As<IPluginRepository>()
                        .As<ISatisfyPluginRequests>();
                }

                var container = builder.Build();

                Func<IBuildFixedSchedules> scheduleBuilder = () => container.Resolve<IBuildFixedSchedules>();
                return new RemoteAssemblyScanner(
                    container.Resolve<IPluginRepository>(),
                    container.Resolve<IConnectParts>(),
                    logger,
                    scheduleBuilder);
            }
            catch (Exception e)
            {
                logger.Log(LogSeverityProxy.Error, e.ToString());
                throw;
            }
        }
    }
}
