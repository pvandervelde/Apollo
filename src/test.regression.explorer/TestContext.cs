//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Test.Regression.Explorer.Controls;
using TestStack.White;

namespace Test.Regression.Explorer
{
    internal sealed class TestContext : IDisposable
    {
        /// <summary>
        /// The application for the current test.
        /// </summary>
        private readonly Application m_Application;

        /// <summary>
        /// The log for the current test.
        /// </summary>
        private readonly Log m_Log;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestContext"/> class.
        /// </summary>
        /// <param name="path">The full path to the application executable.</param>
        /// <param name="log">The log for the test.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="log"/> is <see langword="null" />.
        /// </exception>
        public TestContext(string path, Log log)
        {
            {
                Lokad.Enforce.Argument(() => path);
                Lokad.Enforce.Argument(() => path, Lokad.Rules.StringIs.NotEmpty);
                Lokad.Enforce.Argument(() => log);
            }

            m_Log = log;
            m_Application = ApplicationProxies.StartApplication(path, log);
            if ((m_Application == null) || (m_Application.Process == null) || Application.Process.HasExited)
            {
                throw new RegressionTestFailedException();
            }

            var text = string.Format(
                CultureInfo.InvariantCulture,
                "Started [{0}] - PID: [{1}]",
                m_Application.Name,
                m_Application.Process.Id);
            log.Info("TestContext - Create", text);
        }

        /// <summary>
        /// Gets the application object for the test.
        /// </summary>
        public Application Application
        {
            get
            {
                return m_Application;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            const string prefix = "TestContext - Dispose";
            try
            {
                MenuProxies.CloseApplicationViaFileExitMenuItem(m_Application, m_Log);
                m_Application.Process.WaitForExit(10000);
            }
            catch (Exception e)
            {
                m_Log.Error(
                    prefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to terminate application via the menu. Error was: {0}",
                        e));
            }

            try
            {
                if ((m_Application != null) && (m_Application.Process != null) && !m_Application.Process.HasExited)
                {
                    ApplicationProxies.ExitApplication(m_Application, m_Log);
                }
            }
            catch (Exception e)
            {
                m_Log.Error(
                    prefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to terminate application. Error was: {0}",
                        e));
            }
        }
    }
}
