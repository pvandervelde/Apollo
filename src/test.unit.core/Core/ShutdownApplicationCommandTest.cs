//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the ShutdownApplicationCommand class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ShutdownApplicationCommandTest
    {
        [Test]
        [Description("Checks that the command can be invoked successfully.")]
        public void Invoke()
        {
            bool wasInvoked = false;

            Action function = () =>
                {
                    wasInvoked = true;
                };
            var command = new ShutdownApplicationCommand(function);

            var context = new ShutdownApplicationContext();
            command.Invoke(context);
            Assert.IsTrue(wasInvoked);
        }
    }
}
