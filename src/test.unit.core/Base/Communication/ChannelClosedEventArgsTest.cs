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
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ChannelClosedEventArgsTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("a");
            var args = new ChannelClosedEventArgs(id);

            Assert.AreSame(id, args.ClosedChannel);
        }
    }
}
