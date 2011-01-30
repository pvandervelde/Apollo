//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.UserInterfaces.Projects
{
    [TestFixture]
    [Description("Tests the UnloadProjectCommand class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class UnloadProjectCommandTest
    {
        [Test]
        [Description("Checks that the command can be invoked successfully.")]
        public void Invoke()
        {
            bool wasInvoked = false;

            Action action = () =>
            {
                wasInvoked = true;
            };
            
            var command = new UnloadProjectCommand(action);
            var context = new UnloadProjectContext();
            command.Invoke(context);

            context.Result.Wait();
            Assert.IsTrue(wasInvoked);
        }
    }
}