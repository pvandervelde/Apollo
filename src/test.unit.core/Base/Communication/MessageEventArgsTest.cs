//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the MessageEventArgs class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class MessageEventArgsTest
    {
        [Test]
        [Description("Checks that an object can be created.")]
        public void Create()
        {
            var msg = new Mock<ICommunicationMessage>().Object;
            var args = new MessageEventArgs(msg);

            Assert.AreSame(msg, args.Message);
        }
    }
}
