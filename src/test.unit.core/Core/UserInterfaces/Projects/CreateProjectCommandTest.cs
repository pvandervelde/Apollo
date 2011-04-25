//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Projects;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.UserInterfaces.Projects
{
    [TestFixture]
    [Description("Tests the CreateProjectCommand class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CreateProjectCommandTest
    {
        [Test]
        [Description("Checks that the command can be invoked successfully.")]
        public void Invoke()
        {
            Func<IProject> function = () =>
            {
                return new Mock<IProject>().Object;
            };
            var command = new CreateProjectCommand(function);

            var context = new CreateProjectContext();
            command.Invoke(context);
            Assert.IsNotNull(context.Result);
        }
    }
}