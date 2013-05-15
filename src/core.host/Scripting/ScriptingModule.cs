//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Autofac;

namespace Apollo.Core.Host.Scripting
{
    /// <summary>
    /// Handles the component registrations for the User Interface part 
    /// of the core.
    /// </summary>
    internal sealed class ScriptingModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new ScriptHost(
                    c.Resolve<ILinkToProjects>(),
                    c.Resolve<Func<string, AppDomainPaths, AppDomain>>()))
                .As<IHostScripts>()
                .SingleInstance();

            builder.Register(c => new ScriptOutputPipe())
                .As<ISendScriptOutput>();
        }
    }
}
