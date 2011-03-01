//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the ChannelClosedEventArgs class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ChannelClosedEventArgsTest
    {
        [Test]
        [Description("Checks that an object cannot be created without an endpoint ID.")]
        public void CreateWithNullId()
        {
            Assert.Throws<ArgumentNullException>(() => new ChannelClosedEventArgs(null));
        }

        [Test]
        [Description("Checks that an object can be created.")]
        public void Create()
        {
            var id = new EndpointId("a");
            var args = new ChannelClosedEventArgs(id);

            Assert.AreSame(id, args.ClosedChannel);
        }
    }
}
