﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Apollo.Core.Base.Loaders;
using Autofac;
using Nuclei;
using Nuclei.Diagnostics;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registrations for the loader components that
    /// are used by applications that start dataset applications.
    /// </summary>
    public sealed class BaseModuleForLoaders : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new DatasetApplicationLoader(
                    c.Resolve<IApplicationConstants>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IApplicationLoader>();

            builder.Register(c => new DatasetDistributionGenerator(
                    c.Resolve<IEnumerable<IGenerateDistributionProposals>>()))
                .As<IHelpDistributingDatasets>();

            builder.Register(c => new DistributionCalculator())
                .As<ICalculateDistributionParameters>();
        }
    }
}
