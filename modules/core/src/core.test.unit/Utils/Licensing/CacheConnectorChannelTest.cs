//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils.Licensing;
using MbUnit.Framework;

namespace Apollo.Core.Utils.Licensing
{
    [TestFixture]
    [Description("Tests the CacheConnectorChannel class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CacheConnectorChannelTest
    {
        #region internal class - MockLicenseValidationCacheProxy

        private sealed class MockLicenseValidationCacheProxy : ILicenseValidationCacheProxy
        {
            public LicenseCheckResult LatestResult
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        #endregion

        #region internal class - MockCacheConnectorChannelEndPoint

        private sealed class MockCacheConnectorChannelEndPoint : ICacheConnectorChannelEndPoint
        {
            private ILicenseValidationCacheProxy m_Proxy;

            public MockCacheConnectorChannelEndPoint()
                : this(new MockLicenseValidationCacheProxy())
            {
            }

            public MockCacheConnectorChannelEndPoint(ILicenseValidationCacheProxy proxy)
            {
                m_Proxy = proxy;
            }

            public ILicenseValidationCacheProxy LocalProxy()
            {
                return m_Proxy;
            }

            public void Connect(AppDomain cacheDomain, ILicenseValidationCacheProxy proxy)
            {
                Domain = cacheDomain;
                Proxy = proxy;
            }

            public void Disconnect(AppDomain cacheDomain)
            {
                Domain = cacheDomain;
                Proxy = null;
            }

            public AppDomain Domain
            {
                get;
                set;
            }

            public ILicenseValidationCacheProxy Proxy
            {
                get;
                set;
            }
        }
        
        #endregion

        [Test]
        [Description("Checks that a channel cannot connect to an endpoint with a null AppDomain reference.")]
        public void ConnectToWithNullAppDomain()
        { 
            var channel = new CacheConnectorChannel();
            Assert.Throws<ArgumentNullException>(() => channel.ConnectTo(null, new MockCacheConnectorChannelEndPoint()));
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
            var endPoint1 = new MockCacheConnectorChannelEndPoint();
            var endPoint2 = new MockCacheConnectorChannelEndPoint();
            var channel = new CacheConnectorChannel();
            
            channel.ConnectTo(AppDomain.CurrentDomain, endPoint1);
            channel.ConnectTo(AppDomain.CurrentDomain, endPoint2);

            Assert.IsNull(endPoint1.Proxy);
            Assert.IsNull(endPoint2.Proxy);
        }

        [Test]
        [Description("Checks that a channel correctly connects multiple endpoints.")]
        public void ConnectToWithStoredEndPoints()
        {
            var endPoint1 = new MockCacheConnectorChannelEndPoint();
            var endPoint2 = new MockCacheConnectorChannelEndPoint();
            var domain1 = AppDomain.CreateDomain("domain1");
            var domain2 = AppDomain.CreateDomain("domain2");
            
            var channel = new CacheConnectorChannel();

            channel.ConnectTo(domain1, endPoint1);
            channel.ConnectTo(domain2, endPoint2);

            Assert.AreSame(endPoint2.LocalProxy(), endPoint1.Proxy);
            Assert.AreSame(endPoint1.LocalProxy(), endPoint2.Proxy);
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
            var endPoint1 = new MockCacheConnectorChannelEndPoint();
            var endPoint2 = new MockCacheConnectorChannelEndPoint();
            var domain1 = AppDomain.CreateDomain("domain1");
            var domain2 = AppDomain.CreateDomain("domain2");

            var channel = new CacheConnectorChannel();

            channel.ConnectTo(domain1, endPoint1);
            channel.ConnectTo(domain2, endPoint2);
            channel.DisconnectFrom(AppDomain.CurrentDomain);

            Assert.AreSame(endPoint2.LocalProxy(), endPoint1.Proxy);
            Assert.AreSame(endPoint1.LocalProxy(), endPoint2.Proxy);
        }

        [Test]
        [Description("Checks that a channel correctly disconnects from an existing endpoint.")]
        public void DisconnectFromWithMultipleEndPoints()
        {
            var endPoint1 = new MockCacheConnectorChannelEndPoint();
            var endPoint2 = new MockCacheConnectorChannelEndPoint();
            var domain1 = AppDomain.CreateDomain("domain1");
            var domain2 = AppDomain.CreateDomain("domain2");

            var channel = new CacheConnectorChannel();

            channel.ConnectTo(domain1, endPoint1);
            channel.ConnectTo(domain2, endPoint2);
            channel.DisconnectFrom(domain1);

            Assert.IsNull(endPoint1.Proxy);
            Assert.IsNull(endPoint2.Proxy);
        }
    }
}
