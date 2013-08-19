//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems.Finders;

namespace Test.Regression.Explorer.Automation.Views
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ShellTest
    {
        private int m_ProcessId;

        [Test]
        public void StartApplication()
        {
            var processInfo = new ProcessStartInfo
                {
                    FileName = string.Empty,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Maximized,
                };

            var application = Application.Launch(processInfo);
            m_ProcessId = application.Process.Id;

            var searchCriteria = SearchCriteria.ByText(string.Empty);
            var mainWindow = application.GetWindow(searchCriteria, InitializeOption.NoCache);

            Assert.IsNotNull(mainWindow);
            Assert.IsTrue(mainWindow.IsCurrentlyActive);
        }

        [Test]
        public void ExitApplication()
        {
            Assert.Less(-1, m_ProcessId);

            var application = Application.Attach(m_ProcessId);
            application.Close();

            Assert.IsTrue(application.HasExited);
        }
    }
}
