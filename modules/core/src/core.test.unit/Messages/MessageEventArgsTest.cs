//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messages;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Test.Unit.Messages
{
    [TestFixture]
    [Description("Tests the MessageEventArgs class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MessageEventArgsTest
    {
        [Test]
        [Description("Checks that the MessageEventArgs constructor stores the values correctly.")]
        public void Create()
        {
            var id = MessageId.Next();
            var mockReason = new Mock<IDeliveryFailureReason>();

            var args = new MessageEventArgs(id, mockReason.Object);

            Assert.AreEqual(id, args.Id);
            Assert.AreEqual(mockReason.Object, args.FailureReason);
        }
    }
}
