﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ManualDiscoverySourceTest
    {
        [Test]
        public void StartDiscovery()
        {
            var source = new ManualDiscoverySource();

            EndpointId connectedEndpoint = null;
            Type channelType = null;
            Uri channelUri = null;
            source.OnEndpointBecomingAvailable +=
                (s, e) =>
                {
                    connectedEndpoint = e.ConnectionInformation.Id;
                    channelType = e.ConnectionInformation.ChannelType;
                    channelUri = e.ConnectionInformation.Address;
                };

            source.RecentlyConnectedEndpoint(new EndpointId("endpoint"), typeof(TcpChannelType), new Uri("net.pipe://localhost/apollo_test"));
            Assert.IsNull(connectedEndpoint);

            source.StartDiscovery();

            var newEndpoint = new EndpointId("other");
            var type = typeof(TcpChannelType);
            var uri = new Uri("net.pipe://localhost/apollo_test");
            source.RecentlyConnectedEndpoint(newEndpoint, type, uri);

            Assert.AreSame(newEndpoint, connectedEndpoint);
            Assert.AreSame(type, channelType);
            Assert.AreSame(uri, channelUri);
        }

        [Test]
        public void EndDiscovery()
        {
            var source = new ManualDiscoverySource();
            source.StartDiscovery();

            EndpointId connectedEndpoint = null;
            Type channelType = null;
            Uri channelUri = null;
            source.OnEndpointBecomingAvailable +=
                (s, e) =>
                {
                    connectedEndpoint = e.ConnectionInformation.Id;
                    channelType = e.ConnectionInformation.ChannelType;
                    channelUri = e.ConnectionInformation.Address;
                };

            var newEndpoint = new EndpointId("other");
            var type = typeof(TcpChannelType);
            var uri = new Uri("net.pipe://localhost/apollo_test");
            source.RecentlyConnectedEndpoint(newEndpoint, type, uri);

            Assert.AreSame(newEndpoint, connectedEndpoint);
            Assert.AreSame(type, channelType);
            Assert.AreSame(uri, channelUri);

            source.EndDiscovery();
            connectedEndpoint = null;
            channelType = null;
            channelUri = null;

            source.RecentlyConnectedEndpoint(newEndpoint, type, uri);
            Assert.IsNull(connectedEndpoint);
            Assert.IsNull(channelType);
            Assert.IsNull(channelUri);
        }

        [Test]
        public void RecentlyConnectedEndpoint()
        {
            var source = new ManualDiscoverySource();
            source.StartDiscovery();

            EndpointId connectedEndpoint = null;
            Type channelType = null;
            Uri channelUri = null;
            source.OnEndpointBecomingAvailable +=
                (s, e) =>
                {
                    connectedEndpoint = e.ConnectionInformation.Id;
                    channelType = e.ConnectionInformation.ChannelType;
                    channelUri = e.ConnectionInformation.Address;
                };

            var newEndpoint = new EndpointId("other");
            var type = typeof(TcpChannelType);
            var uri = new Uri("net.pipe://localhost/apollo_test");
            source.RecentlyConnectedEndpoint(newEndpoint, type, uri);

            Assert.AreSame(newEndpoint, connectedEndpoint);
            Assert.AreSame(type, channelType);
            Assert.AreSame(uri, channelUri);
        }

        [Test]
        public void RecentlyDisconnectedEndpoint()
        {
            var source = new ManualDiscoverySource();
            source.StartDiscovery();

            EndpointId disconnectedEndpoint = null;
            source.OnEndpointBecomingUnavailable +=
                (s, e) =>
                {
                    disconnectedEndpoint = e.Endpoint;
                };

            var newEndpoint = new EndpointId("other");
            source.RecentlyDisconnectedEndpoint(newEndpoint);

            Assert.AreSame(newEndpoint, disconnectedEndpoint);
        }
    }
}