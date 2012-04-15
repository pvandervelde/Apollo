//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Apollo.UI.Common.Feedback;
using Apollo.UI.Common.Profiling;
using Apollo.Utilities;
using Apollo.Utilities.Configuration;
using Autofac;
using NManto;
using NManto.Reporting;
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

                builder.Register(c => new TimingReportCollection())
                    .SingleInstance();

                builder.Register<Func<string, IDisposable>>(
                    c => 
                    {
                        Func<string, IDisposable> result = description => null;
                        if (ConfigurationHelpers.ShouldBeProfiling())
                        {
                            // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                            // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                            var ctx = c.Resolve<IComponentContext>();
                            Func<Report, string> reportBuilder =
                                report =>
                                {
                                    var stream = new MemoryStream();
                                    Func<Stream> streamFactory = () => stream;
                                    var transformer = ctx.Resolve<TextReporter>(
                                        new Autofac.Core.Parameter[] 
                                    { 
                                        new TypedParameter(typeof(Func<Stream>), streamFactory) 
                                    });
                                    transformer.Transform(report);

                                    // normally you can't touch the stream anymore, but it turns out you
                                    // can touch the actual buffer
                                    return Encoding.Unicode.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                                };

                            result =
                                description =>
                                    new TimingIntervalHelper(
                                        ctx.Resolve<SystemDiagnostics>(),
                                        ctx.Resolve<TimingStorage>(),
                                        ctx.Resolve<TimingReportCollection>(),
                                        reportBuilder,
                                        description);
                        }

                        return result;
                    })
                    .SingleInstance();
            }
        }
    }
}
