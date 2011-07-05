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
    public sealed class ConnectionInformationEventArgsTest
    {
        [Test]
        public void Create()
        {
            var endpoint = new EndpointId("id");
            var channelType = typeof(TcpChannelType);
            var uri = new Uri("http://localhost");

            var info = new ChannelConnectionInformation(endpoint, channelType, uri);
            var args = new ConnectionInformationEventArgs(info);

            Assert.AreSame(info, args.ConnectionInformation);
        }
    }
}
