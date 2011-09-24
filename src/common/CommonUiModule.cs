//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Common.Feedback;
using Apollo.Utilities;
using Autofac;
using NSarrac.Framework;

namespace Apollo.UI.Common
{
    /// <summary>
    /// Handles the component registrations for the Apollo.UI.Common assembly.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class CommonUIModule : Module
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
                builder.Register(c => new FeedbackReportCollector(
                        c.Resolve<IFileConstants>()))
                   .As<ICollectFeedbackReports>();

                builder.Register(c => new FeedbackReportTransmitter(
                        c.Resolve<IReportSender>()))
                    .As<ISendFeedbackReports>();
            }
        }
    }
}
