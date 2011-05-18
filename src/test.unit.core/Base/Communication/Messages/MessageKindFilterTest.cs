//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Communication.Messages;
using MbUnit.Framework;

namespace Apollo.Base.Communication.Messages
{
    [TestFixture]
    [Description("Tests the MessageKindFilter class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class MessageKindFilterTest
    {
        [Test]
        [Description("Checks that an correct message passes the filter.")]
        public void PassThroughWithPassingMessage()
        {
            var filter = new MessageKindFilter(typeof(SuccessMessage));
            Assert.IsTrue(filter.PassThrough(new SuccessMessage(new EndpointId("id"), new MessageId())));
        }

        [Test]
        [Description("Checks that an incorrect message does not pass the filter.")]
        public void PassThroughWithNonPassingMessage()
        {
            var filter = new MessageKindFilter(typeof(SuccessMessage));
            Assert.IsFalse(filter.PassThrough(new FailureMessage(new EndpointId("id"), new MessageId())));
        }
    }
}
