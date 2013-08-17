//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Test.Regression.Console;

namespace Test.Integration.Console.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class Project
    {
        [Test]
        public void NewProject()
        {
        }

        [Test]
        public void LoadProject()
        {
            var application = ApplicationHelpers.StartConsole(string.Empty);

            application.OutputDataReceived += null;
            application.ErrorDataReceived += null;

            application.Exited += null;
        }

        [Test]
        public void UpdateProject()
        {
        }
    }
}
