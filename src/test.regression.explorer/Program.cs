//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Autofac;
using Castle.Core.Logging;
using Test.Regression.Explorer.Controls;
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

        /// <summary>
        /// The maximum number of times we retry the test.
        /// </summary>
        private const int MaximumRetryCount = 3;

        static int Main(string[] args)
        {
            InitializeWhite();

            // Initialize the container
            var container = DependencyInjection.Load();

            var log = container.Resolve<Log>();
            var globalResult = new TestResult();
            
            var applicationPath = ApplicationProxies.GetApolloExplorerPath(log);
            if (string.IsNullOrEmpty(applicationPath))
            {
                throw new RegressionTestFailedException("Could not find application path.");
            }

            // Select the type of test to execute
            var verifier = container.ResolveKeyed<IUserInterfaceVerifier>(typeof(VerifyViews));
            foreach (var testCase in verifier.TestsToExecute())
            {
                var localResult = ExecuteTestCase(testCase, log, applicationPath);
                foreach (var error in localResult.Errors)
                {
                    globalResult.AddError(error);
                }
            }

            // Write a report with all the errors.
            foreach (var error in globalResult.Errors)
            {
                Console.Error.WriteLine(error);
            }

            Console.ReadLine();

            return globalResult.Status == TestStatus.Passed ? NormalApplicationExitCode : UnhandledExceptionApplicationExitCode;
        }

        private static void InitializeWhite()
        {
            // Set the search depth, we won't go more than two levels down in controls.
            CoreAppXmlConfiguration.Instance.RawElementBasedSearch = true;
            CoreAppXmlConfiguration.Instance.MaxElementSearchDepth = 2;

            // Don't log anything for the moment.
            CoreAppXmlConfiguration.Instance.LoggerFactory = new WhiteDefaultLoggerFactory(LoggerLevel.Error);
        }

        private static TestResult ExecuteTestCase(TestCase testCase, Log testLog, string applicationPath)
        {
            var count = 0;
            var hasPassed = false;
            var result = new TestResult();
            while ((count < MaximumRetryCount) && (!hasPassed))
            {
                testLog.Info(
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
                                result.AddError(error);
                            }
                        }
                        catch (Exception e)
                        {
                            testLog.Error(e.ToString());
                            result.AddError(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Error in test case: {0}. Error was: {1}",
                                    testCase.Name,
                                    e));
                        }
                        finally
                        {
                            testLog.Info(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Completed test case: {0}. Result: {1}",
                                    testCase.Name,
                                    result.Status));
                        }
                    }
                }
                catch (Exception e)
                {
                    result.AddError(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Test case failed for: {0}. Iteration: {1}. Error: {2}",
                            testCase.Name,
                            count + 1,
                            e));
                }

                hasPassed = result.Status == TestStatus.Passed;
                count++;
            }

            return result;
        }
    }
}
