//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Apollo.UI.Wpf.Feedback;
using Apollo.UI.Wpf.Profiling;
using Apollo.Utilities;
using Autofac;
using NSarrac.Framework;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Profiling;
using Nuclei.Diagnostics.Profiling.Reporting;

namespace Apollo.UI.Wpf
{
    /// <summary>
    /// Handles the component registrations for the Apollo.UI.Wpf assembly.
    /// </summary>
    public sealed class CommonUIModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults).
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register the global application objects
            {
                builder.Register(c => new DependencyInjectionProxy(
                        c.Resolve<IContainer>()))
                    .As<IDependencyInjectionProxy>();

                builder.Register(c => new FeedbackReportCollector(
                        c.Resolve<IFileSystem>(),
                        c.Resolve<FileConstants>()))
                   .As<ICollectFeedbackReports>();

                builder.Register(c => new FeedbackReportTransmitter(
                        c.Resolve<IReportSender>()))
                    .As<ISendFeedbackReports>();

                builder.Register(c => new TimingReportCollection())
                    .SingleInstance();

                builder.Register(
                    c => 
                    {
                        Func<string, IDisposable> result = description => null;
                        if (ConfigurationHelpers.ShouldBeProfiling())
                        {
                            // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                            // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                            var ctx = c.Resolve<IComponentContext>();
                            Func<TimingReport, string> reportBuilder =
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
                                    var buffer = stream.GetBuffer();
                                    var outputString = Encoding.Unicode.GetString(buffer, 0, buffer.Length);

                                    // Note that the result may have terminating characters, multiple of them
                                    // because we don't know the amount of data written to the buffer.
                                    return outputString.Replace("\0", string.Empty);
                                };

                            result =
                                description =>
                                    new TimingIntervalHelper(
                                        ctx.Resolve<SystemDiagnostics>(),
                                        ctx.Resolve<IGenerateTimingReports>(),
                                        ctx.Resolve<TimingReportCollection>(),
                                        reportBuilder,
                                        description);
                        }

                        return result;
                    })
                    .SingleInstance();

                builder.Register(c => new FileSystem())
                    .As<IFileSystem>();
            }
        }
    }
}
