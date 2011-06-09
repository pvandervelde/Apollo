//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.UI.Common.Scripting;
using Apollo.Utilities;
using Autofac;

namespace Apollo.UI.Common
{
    /// <summary>
    /// Handles the component registrations for the UI.Common assembly.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class CommonUserInterfaceModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults.)
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register the global application objects
            {
                builder.Register(c => new ScriptHost(
                        c.Resolve<ILinkToProjects>(),
                        c.Resolve<Func<string, AppDomainPaths, AppDomain>>()))
                    .As<IHostScripts>()
                    .SingleInstance();
            }
        }
    }
}
