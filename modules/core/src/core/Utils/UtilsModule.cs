//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utils;
using Autofac;

namespace Apollo.Core.Utils
{
    /// <summary>
    /// Handles the component registrations for the utilities part 
    /// of the core.
    /// </summary>
    internal sealed class UtilsModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="moduleBuilder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults.)
        /// </remarks>
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            base.Load(moduleBuilder);

            moduleBuilder.Register(c => new ApplicationConstants())
                .As<IApplicationConstants>()
                .As<ICompanyConstants>();

            moduleBuilder.Register(c => new FileConstants(c.Resolve<IApplicationConstants>()))
                .As<IFileConstants>();
        }
    }
}
