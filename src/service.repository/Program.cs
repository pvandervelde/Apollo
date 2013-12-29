//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.Service.Repository.Nuclei.ExceptionHandling;
using Apollo.Service.Repository.Properties;
using Apollo.Utilities;
using Nuclei.Configuration;
using Nuclei.Diagnostics.Logging;
using Topshelf;
using Topshelf.ServiceConfigurators;

namespace Apollo.Service.Repository
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        private const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        private const int UnhandledExceptionApplicationExitCode = 1;

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "service.repository.error.{0}.log";

        [STAThread]
        static int Main()
        {
            int functionReturnResult = -1;

            using (var processor = new LogBasedExceptionProcessor(
                LoggerBuilder.ForFile(
                    Path.Combine(new FileConstants(new ApplicationConstants()).LogPath(), DefaultErrorFileName),
                    new DebugLogTemplate(new NullConfiguration(), () => DateTimeOffset.Now))))
            {
                var result = TopLevelExceptionGuard.RunGuarded(
                () => functionReturnResult = RunApplication(),
                new ExceptionProcessor[]
                    {
                        processor.Process,
                    });

                return (result == GuardResult.Failure) ? UnhandledExceptionApplicationExitCode : functionReturnResult;
            }
        }

        private static int RunApplication()
        {
            var host = HostFactory.New(
                c =>
                {
                    c.Service(
                        (ServiceConfigurator<ServiceEntryPoint> s) =>
                        {
                            s.ConstructUsing(() => new ServiceEntryPoint());
                            s.WhenStarted(m => m.OnStart());
                            s.WhenStopped(m => m.OnStop());
                        });
                    c.RunAsNetworkService();
                    c.StartAutomaticallyDelayed();

                    c.DependsOnEventLog();

                    c.EnableShutdown();

                    c.SetServiceName(Resources.Service_ServiceName);
                    c.SetDisplayName(Resources.Service_DisplayName);
                    c.SetDescription(Resources.Service_Description);
                });

            var exitCode = host.Run();
            return (exitCode == TopshelfExitCode.Ok)
                ? NormalApplicationExitCode
                : UnhandledExceptionApplicationExitCode;
        }
    }
}
