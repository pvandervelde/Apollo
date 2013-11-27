//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Castle.Core.Logging;
using Test.Regression.Explorer.UseCases;
using TestStack.White.Configuration;

namespace Test.Regression.Explorer
{
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

        static int Main(string[] args)
        {
            InitializeWhite();

            // Initialize the container
            var container = DependencyInjection.Load();

            // Select the type of test to execute
            // Initialize the correct verifier
            var verifier = container.ResolveKeyed<IUserInterfaceVerifier>(typeof(VerifyViews));

            // Execute
            var log = container.Resolve<Log>();
            verifier.Verify(log);

            Console.ReadLine();

            var result = container.Resolve<TestResult>();
            return result.Status == TestStatus.Passed ? NormalApplicationExitCode : UnhandledExceptionApplicationExitCode;
        }

        private static void InitializeWhite()
        {
            // Set the search depth, we won't go more than two levels down in controls.
            CoreAppXmlConfiguration.Instance.RawElementBasedSearch = true;
            CoreAppXmlConfiguration.Instance.MaxElementSearchDepth = 2;

            // Don't log anything for the moment.
            CoreAppXmlConfiguration.Instance.LoggerFactory = new WhiteDefaultLoggerFactory(LoggerLevel.Error);
        }
    }
}
