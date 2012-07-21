//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace Apollo.Core.Extensions.Scheduling
{
    /// <summary>
    /// Handles the component registrations for the communication and loader components.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class SchedulingModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new FixedScheduleBuilder())
                .As<IBuildFixedSchedules>();

            builder.Register(c => new ScheduleVerifier(
                    c.Resolve<IStoreSchedules>()))
                .As<IVerifyScheduleIntegrity>();
        }
    }
}
