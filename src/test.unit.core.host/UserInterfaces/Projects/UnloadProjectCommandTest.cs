//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class UnloadProjectCommandTest
    {
        [Test]
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
