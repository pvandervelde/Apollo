//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Base.Communication.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class MessageKindFilterTest
    {
        [Test]
        public void PassThroughWithPassingMessage()
        {
            var filter = new MessageKindFilter(typeof(SuccessMessage));
            Assert.IsTrue(filter.PassThrough(new SuccessMessage(new EndpointId("id"), new MessageId())));
        }

        [Test]
        public void PassThroughWithNonPassingMessage()
        {
            var filter = new MessageKindFilter(typeof(SuccessMessage));
            Assert.IsFalse(filter.PassThrough(new FailureMessage(new EndpointId("id"), new MessageId())));
        }
    }
}
