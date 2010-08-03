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
    [Description("Tests the CacheConnectorChannelEndPoint class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CacheConnectorChannelEndPointTest
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

        #region internal class - MockCacheProxyHolder

        private sealed class MockCacheProxyHolder : ICacheProxyHolder
        {
            public void Store(ILicenseValidationCacheProxy proxy)
            {
                Proxy = proxy;
            }

            public void Release(ILicenseValidationCacheProxy proxy)
            {
                if (ReferenceEquals(Proxy, proxy))
                {
                    Proxy = null;
                }
            }

            public ILicenseValidationCacheProxy Proxy
            {
                get;
                set;
            }
        }
        
        #endregion

        [Test]
        [Description("Checks that an EndPoint cannot be created with a null factory reference.")]
        public void CreateWithNullProxyFactory()
        { 
            Assert.Throws<ArgumentNullException>(() => new CacheConnectorChannelEndPoint(null, new MockCacheProxyHolder()));
        }

        [Test]
        [Description("Checks that an EndPoint cannot be created with a null proxy holder.")]
        public void CreateWithNullCache()
        {
            Assert.Throws<ArgumentNullException>(() => new CacheConnectorChannelEndPoint(() => new MockLicenseValidationCacheProxy(), null));
        }

        [Test]
        [Description("Checks that an EndPoint returns the correct local proxy value.")]
        public void LocalProxy()
        {
            var proxy = new MockLicenseValidationCacheProxy();
            var endPoint = new CacheConnectorChannelEndPoint(() => proxy, new MockCacheProxyHolder());

            Assert.AreSame(proxy, endPoint.LocalProxy());
        }

        [Test]
        [Description("Checks that an EndPoint cannot connect to a proxy with a null AppDomain reference.")]
        public void ConnectWithNullAppDomain()
        {
            var proxy = new MockLicenseValidationCacheProxy();
            var holder = new MockCacheProxyHolder();
            var endPoint = new CacheConnectorChannelEndPoint(() => proxy, holder);

            Assert.Throws<ArgumentNullException>(() => endPoint.Connect(null, new MockLicenseValidationCacheProxy()));
        }

        [Test]
        [Description("Checks that an EndPoint cannot connect to a proxy with a null proxy reference.")]
        public void ConnectWithNullProxy()
        { 
            var proxy = new MockLicenseValidationCacheProxy();
            var holder = new MockCacheProxyHolder();
            var endPoint = new CacheConnectorChannelEndPoint(() => proxy, holder);

            Assert.Throws<ArgumentNullException>(() => endPoint.Connect(AppDomain.CurrentDomain, null));
        }

        [Test]
        [Description("Checks that an EndPoint does not store a proxy if another proxy is already stored for the given AppDomain.")]
        public void ConnectWithExistingAppDomain()
        {
            var proxy = new MockLicenseValidationCacheProxy();
            var holder = new MockCacheProxyHolder();
            var endPoint = new CacheConnectorChannelEndPoint(() => proxy, holder);

            endPoint.Connect(AppDomain.CurrentDomain, proxy);
            endPoint.Connect(AppDomain.CurrentDomain, new MockLicenseValidationCacheProxy());

            Assert.AreSame(proxy, holder.Proxy);
        }

        [Test]
        [Description("Checks that an EndPoint can be connected to a proxy.")]
        public void Connect()
        {
            var proxy = new MockLicenseValidationCacheProxy();
            var holder = new MockCacheProxyHolder();
            var endPoint = new CacheConnectorChannelEndPoint(() => proxy, holder);

            endPoint.Connect(AppDomain.CurrentDomain, proxy);
            Assert.AreSame(proxy, holder.Proxy);
        }

        [Test]
        [Description("Checks that an EndPoint cannot be disconnected with a null AppDomain reference.")]
        public void DisconnectWithNullAppDomain()
        {
            var proxy = new MockLicenseValidationCacheProxy();
            var holder = new MockCacheProxyHolder();
            var endPoint = new CacheConnectorChannelEndPoint(() => proxy, holder);

            Assert.Throws<ArgumentNullException>(() => endPoint.Disconnect(null));
        }

        [Test]
        [Description("Checks that an EndPoint cannot connect to a proxy with a null AppDomain reference.")]
        public void DisconnectWithNonExistingAppDomain()
        {
            var proxy = new MockLicenseValidationCacheProxy();
            var holder = new MockCacheProxyHolder();
            var endPoint = new CacheConnectorChannelEndPoint(() => proxy, holder);

            holder.Proxy = proxy;
            endPoint.Disconnect(AppDomain.CurrentDomain);
            Assert.IsNotNull(holder.Proxy);
        }

        [Test]
        [Description("Checks that an EndPoint cannot be disconnected from a proxy.")]
        public void Disconnect()
        {
            var proxy = new MockLicenseValidationCacheProxy();
            var holder = new MockCacheProxyHolder();
            var endPoint = new CacheConnectorChannelEndPoint(() => proxy, holder);

            endPoint.Connect(AppDomain.CurrentDomain, proxy);
            Assert.AreSame(proxy, holder.Proxy);

            endPoint.Disconnect(AppDomain.CurrentDomain);
            Assert.IsNull(holder.Proxy);
        }
    }
}
