//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils.Licensing;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Utils.Licensing
{
    [TestFixture]
    [Description("Tests the CacheConnectorChannel class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CacheConnectorChannelTest
    {
        [Test]
        [Description("Checks that a channel cannot connect to an endpoint with a null AppDomain reference.")]
        public void ConnectToWithNullAppDomain()
        { 
            var channel = new CacheConnectorChannel();
            Assert.Throws<ArgumentNullException>(() => channel.ConnectTo(null, new Mock<ICacheConnectorChannelEndpoint>().Object));
        }

        [Test]
        [Description("Checks that a channel cannot connect to a null endpoint reference.")]
        public void ConnectToWithNullEndPoint()
        {
            var channel = new CacheConnectorChannel();
            Assert.Throws<ArgumentNullException>(() => channel.ConnectTo(AppDomain.CurrentDomain, null));
        }

        [Test]
        [Description("Checks that a channel does not store multiple endpoints in the same AppDomain.")]
        public void ConnectToWithExistingAppDomain()
        {
            AppDomain endPoint1Domain = null;
            ILicenseValidationCacheProxy endpoint1Proxy = null;
            var endPoint1 = new Mock<ICacheConnectorChannelEndpoint>();
            {
                endPoint1.Setup(e => e.Connect(It.IsAny<AppDomain>(), It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<AppDomain, ILicenseValidationCacheProxy>((a, p) =>
                    {
                        endPoint1Domain = a;
                        endpoint1Proxy = p;
                    });
                endPoint1.Setup(e => e.LocalProxy())
                    .Returns(endpoint1Proxy);
            }

            AppDomain endPoint2Domain = null;
            ILicenseValidationCacheProxy endpoint2Proxy = null;
            var endPoint2 = new Mock<ICacheConnectorChannelEndpoint>();
            {
                endPoint2.Setup(e => e.Connect(It.IsAny<AppDomain>(), It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<AppDomain, ILicenseValidationCacheProxy>((a, p) =>
                    {
                        endPoint2Domain = a;
                        endpoint2Proxy = p;
                    });
                endPoint2.Setup(e => e.LocalProxy())
                    .Returns(endpoint2Proxy);
            }

            var channel = new CacheConnectorChannel();
            
            channel.ConnectTo(AppDomain.CurrentDomain, endPoint1.Object);
            channel.ConnectTo(AppDomain.CurrentDomain, endPoint2.Object);

            Assert.IsNull(endPoint1.Object.LocalProxy());
            Assert.IsNull(endPoint2.Object.LocalProxy());
        }

        [Test]
        [Description("Checks that a channel correctly connects multiple endpoints.")]
        public void ConnectToWithStoredEndPoints()
        {
            AppDomain endPoint1Domain = null;
            ILicenseValidationCacheProxy endpoint1Proxy = null;
            var endPoint1 = new Mock<ICacheConnectorChannelEndpoint>();
            {
                endPoint1.Setup(e => e.Connect(It.IsAny<AppDomain>(), It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<AppDomain, ILicenseValidationCacheProxy>((a, p) => 
                        {
                            endPoint1Domain = a;
                            endpoint1Proxy = p;
                        });
                endPoint1.Setup(e => e.LocalProxy())
                    .Returns(endpoint1Proxy);
            }

            AppDomain endPoint2Domain = null;
            ILicenseValidationCacheProxy endpoint2Proxy = null;
            var endPoint2 = new Mock<ICacheConnectorChannelEndpoint>();
            {
                endPoint2.Setup(e => e.Connect(It.IsAny<AppDomain>(), It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<AppDomain, ILicenseValidationCacheProxy>((a, p) =>
                    {
                        endPoint2Domain = a;
                        endpoint2Proxy = p;
                    });
                endPoint2.Setup(e => e.LocalProxy())
                    .Returns(endpoint2Proxy);
            }

            var domain1 = AppDomain.CreateDomain("domain1");
            var domain2 = AppDomain.CreateDomain("domain2");

            var channel = new CacheConnectorChannel();

            channel.ConnectTo(domain1, endPoint1.Object);
            channel.ConnectTo(domain2, endPoint2.Object);

            Assert.AreSame(endPoint2.Object.LocalProxy(), endpoint1Proxy);
            Assert.AreSame(endPoint1.Object.LocalProxy(), endpoint2Proxy);
        }

        [Test]
        [Description("Checks that a channel cannot disconnect from an endpoint with a null AppDomain reference.")]
        public void DisconnectFromWithNullAppDomain()
        {
            var channel = new CacheConnectorChannel();
            Assert.Throws<ArgumentNullException>(() => channel.DisconnectFrom(null));
        }

        [Test]
        [Description("Checks that a channel cannot disconnect from an endpoint that is not registered.")]
        public void DisconnectFromWithNonExistingAppDomain()
        {
            AppDomain endPoint1Domain = null;
            ILicenseValidationCacheProxy endpoint1Proxy = null;
            var endPoint1 = new Mock<ICacheConnectorChannelEndpoint>();
            {
                endPoint1.Setup(e => e.Connect(It.IsAny<AppDomain>(), It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<AppDomain, ILicenseValidationCacheProxy>((a, p) =>
                    {
                        endPoint1Domain = a;
                        endpoint1Proxy = p;
                    });
                endPoint1.Setup(e => e.LocalProxy())
                    .Returns(endpoint1Proxy);
            }

            AppDomain endPoint2Domain = null;
            ILicenseValidationCacheProxy endpoint2Proxy = null;
            var endPoint2 = new Mock<ICacheConnectorChannelEndpoint>();
            {
                endPoint2.Setup(e => e.Connect(It.IsAny<AppDomain>(), It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<AppDomain, ILicenseValidationCacheProxy>((a, p) =>
                    {
                        endPoint2Domain = a;
                        endpoint2Proxy = p;
                    });
                endPoint2.Setup(e => e.LocalProxy())
                    .Returns(endpoint2Proxy);
            }

            var domain1 = AppDomain.CreateDomain("domain1");
            var domain2 = AppDomain.CreateDomain("domain2");

            var channel = new CacheConnectorChannel();

            channel.ConnectTo(domain1, endPoint1.Object);
            channel.ConnectTo(domain2, endPoint2.Object);
            channel.DisconnectFrom(AppDomain.CurrentDomain);

            Assert.AreSame(endPoint2.Object.LocalProxy(), endpoint1Proxy);
            Assert.AreSame(endPoint1.Object.LocalProxy(), endpoint2Proxy);
        }

        [Test]
        [Description("Checks that a channel correctly disconnects from an existing endpoint.")]
        public void DisconnectFromWithMultipleEndPoints()
        {
            AppDomain endPoint1Domain = null;
            ILicenseValidationCacheProxy endpoint1Proxy = null;
            var endPoint1 = new Mock<ICacheConnectorChannelEndpoint>();
            {
                endPoint1.Setup(e => e.Connect(It.IsAny<AppDomain>(), It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<AppDomain, ILicenseValidationCacheProxy>((a, p) =>
                    {
                        endPoint1Domain = a;
                        endpoint1Proxy = p;
                    });
                endPoint1.Setup(e => e.LocalProxy())
                    .Returns(endpoint1Proxy);
            }

            AppDomain endPoint2Domain = null;
            ILicenseValidationCacheProxy endpoint2Proxy = null;
            var endPoint2 = new Mock<ICacheConnectorChannelEndpoint>();
            {
                endPoint2.Setup(e => e.Connect(It.IsAny<AppDomain>(), It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<AppDomain, ILicenseValidationCacheProxy>((a, p) =>
                    {
                        endPoint2Domain = a;
                        endpoint2Proxy = p;
                    });
                endPoint2.Setup(e => e.LocalProxy())
                    .Returns(endpoint2Proxy);
            }

            var domain1 = AppDomain.CreateDomain("domain1");
            var domain2 = AppDomain.CreateDomain("domain2");

            var channel = new CacheConnectorChannel();

            channel.ConnectTo(domain1, endPoint1.Object);
            channel.ConnectTo(domain2, endPoint2.Object);
            channel.DisconnectFrom(domain1);

            Assert.IsNull(endPoint1.Object.LocalProxy());
            Assert.IsNull(endPoint2.Object.LocalProxy());
        }
    }
}
