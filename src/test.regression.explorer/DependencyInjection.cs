//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Autofac;
using NLog;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Test.Regression.Explorer.UseCases;

namespace Test.Regression.Explorer
{
    internal static class DependencyInjection
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultInfoFileName = "test.regression.explorer.log";

        private static void RegisterDiagnostics(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var loggers = c.Resolve<IEnumerable<ILogger>>();
                    Action<LevelToLog, string> action = (p, s) =>
                    {
                        var msg = new LogMessage(p, s);
                        foreach (var logger in loggers)
                        {
                            try
                            {
                                logger.Log(msg);
                            }
                            catch (NLogRuntimeException)
                            {
                                // Ignore it and move on to the next logger.
                            }
                        }
                    };

                    return new SystemDiagnostics(action, null);
                })
                .As<SystemDiagnostics>()
                .SingleInstance();
        }

        private static void RegisterLoggers(ContainerBuilder builder)
        {
            builder.Register(c => LoggerBuilder.ForFile(
                    Path.Combine(
                        Assembly.GetExecutingAssembly().LocalDirectoryPath(),
                        DefaultInfoFileName),
                    new DebugLogTemplate(
                        c.Resolve<IConfiguration>(),
                        () => DateTimeOffset.Now)))
                .As<ILogger>()
                .SingleInstance();

            builder.Register(c => LoggerBuilder.ForEventLog(
                    Assembly.GetExecutingAssembly().GetName().Name,
                    new DebugLogTemplate(
                        c.Resolve<IConfiguration>(),
                        () => DateTimeOffset.Now)))
                .As<ILogger>()
                .SingleInstance();
        }

        private static void RegisterLog(ContainerBuilder builder)
        {
            builder.Register(c => new Log(
                c.Resolve<TestResult>(),
                c.Resolve<SystemDiagnostics>()));
        }

        private static void RegisterTestResult(ContainerBuilder builder)
        {
            builder.Register(c => new TestResult())
                .SingleInstance();
        }

        private static void RegisterTests(ContainerBuilder builder)
        {
            builder.Register(c => new VerifyViews())
                .As<IUserInterfaceVerifier>()
                .Keyed<IUserInterfaceVerifier>(typeof(VerifyViews))
                .SingleInstance();
        }

        /// <summary>
        /// Creates the DI container.
        /// </summary>
        /// <returns>The DI container.</returns>
        public static IContainer Load()
        {
            IContainer result = null;
            var builder = new ContainerBuilder();
            {
                builder.Register(c => new XmlConfiguration(
                        DiagnosticsConfigurationKeys.ToCollection(),
                        RegressionApplicationConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>();

                RegisterLoggers(builder);
                RegisterDiagnostics(builder);
                RegisterTests(builder);
                RegisterLog(builder);
                RegisterTestResult(builder);
            }

            result = builder.Build();
            return result;
        }
    }
}
