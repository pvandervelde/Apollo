//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Autofac;

namespace Apollo.Core.Base.Scheduling
{
    /// <summary>
    /// Handles the component registrations for the scheduling components.
    /// </summary>
    public sealed class SchedulingModule : Module
    {
        private static void RegisterSchedules(ContainerBuilder builder)
        {
            builder.Register(c => new FixedScheduleBuilder())
                .As<IBuildFixedSchedules>();

            builder.Register(c => new ScheduleVerifier(
                    c.Resolve<IStoreSchedules>()))
                .As<IVerifyScheduleIntegrity>();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterSchedules(builder);
        }
    }
}
