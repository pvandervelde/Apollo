//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Autofac;
using Castle.Core.Logging;
using Nuclei.Diagnostics;
using Test.Regression.Explorer.Controls;
using Test.Regression.Explorer.Reporting;
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

        private const int MinimizeWindow = 6;

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow([In]IntPtr hWnd, [In]int nCmdShow);

        private static void MinimizeConsoleWindow()
        {
            IntPtr hWndConsole = GetConsoleWindow();
            ShowWindow(hWndConsole, MinimizeWindow);
        }

        static int Main()
        {
            // Minimize directly on start-up so that we can't take focus on our own window
            MinimizeConsoleWindow();

            var globalResult = new TestResult();
            try
            {
                // Initialize the container
                var container = DependencyInjection.Load();
                InitializeWhite(container);

                var reporters = container.Resolve<IEnumerable<IReporter>>();
                var log = container.Resolve<Log>();

                var applicationPath = ApplicationProxies.GetApolloExplorerPath(log);
                if (string.IsNullOrEmpty(applicationPath))
                {
                    var message = "Could not find application path.";
                    reporters.ForEach(r => r.AddErrorMessage(message));
                    globalResult.AddError(message);
                }

                // Select the type of test to execute
                var views = container.ResolveKeyed<IUserInterfaceVerifier>(typeof(VerifyViews));
                var projects = container.ResolveKeyed<IUserInterfaceVerifier>(typeof(VerifyProject));
                foreach (var testCase in views.TestsToExecute().Append(projects.TestsToExecute()))
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Starting: {0}",
                        testCase.Name);
                    reporters.ForEach(r => r.AddInformationalMessage(message));
                    var localResult = ExecuteTestCase(testCase, log, applicationPath);
                    if (localResult.Status == TestStatus.Passed)
                    {
                        var succesMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            "Successfully completed test: {0}",
                            testCase.Name);
                        reporters.ForEach(r => r.AddInformationalMessage(succesMessage));
                    }
                    else
                    {
                        foreach (var error in localResult.Errors)
                        {
                            var failMessage = error;
                            globalResult.AddError(error);
                            reporters.ForEach(r => r.AddErrorMessage(failMessage));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Unhandled exception occurred during the execution of the regression tests. Error was: {0}",
                    e);
                globalResult.AddError(message);
            }

            return globalResult.Status == TestStatus.Passed ? NormalApplicationExitCode : UnhandledExceptionApplicationExitCode;
        }

        private static void InitializeWhite(IContainer container)
        {
            // Set the search depth, we won't go more than four levels down in controls.
            CoreAppXmlConfiguration.Instance.RawElementBasedSearch = false;
            CoreAppXmlConfiguration.Instance.MaxElementSearchDepth = 4;

            // Don't log anything for the moment.
            CoreAppXmlConfiguration.Instance.LoggerFactory = new WhiteLogRedirectorFactory(
                container.Resolve<SystemDiagnostics>(), 
                LoggerLevel.Debug);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "This is a regression test which should always finish normally.")]
        private static TestResult ExecuteTestCase(TestStep testCase, Log testLog, string applicationPath)
        {
            const string prefix = "Execute test case";
            var count = 0;
            var hasPassed = false;
            var results = Enumerable.Range(0, TestConstants.MaximumRetryCount)
                .Select(i => new TestResult())
                .ToArray();
            while ((count < TestConstants.MaximumRetryCount) && (!hasPassed))
            {
                testLog.Info(
                    prefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Executing test case: {0}. Iteration: {1}",
                        testCase.Name,
                        count + 1));
                try
                {
                    using (var context = new TestContext(applicationPath, testLog))
                    {
                        TestResult local;
                        try
                        {
                            local = testCase.Test(context.Application, testLog);
                            foreach (var error in local.Errors)
                            {
                                results[count].AddError(error);
                            }
                        }
                        catch (Exception e)
                        {
                            testLog.Error(prefix, e.ToString());
                            results[count].AddError(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Error in test case: {0}. Error was: {1}",
                                    testCase.Name,
                                    e));
                        }
                        finally
                        {
                            testLog.Info(
                                prefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Completed test case: {0}. Result: {1}",
                                    testCase.Name,
                                    results[count].Status));
                        }
                    }
                }
                catch (Exception e)
                {
                    results[count].AddError(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Test case failed for: {0}. Iteration: {1}. Error: {2}",
                            testCase.Name,
                            count + 1,
                            e));
                }

                // Take snapshot of application
                // Grab log and error log
                hasPassed = results[count].Status == TestStatus.Passed;
                count++;
            }

            return results[count - 1];
        }
    }
}
