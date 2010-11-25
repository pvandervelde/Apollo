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
    [Description("Tests the CacheConnectorChannelEndPoint class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CacheConnectorChannelEndPointTest
    {
        [Test]
        [Description("Checks that an EndPoint cannot be created with a null factory reference.")]
        public void CreateWithNullProxyFactory()
        { 
            Assert.Throws<ArgumentNullException>(() => new CacheConnectorChannelEndpoint(null, new Mock<ICacheProxyHolder>().Object));
        }

        [Test]
        [Description("Checks that an EndPoint cannot be created with a null proxy holder.")]
        public void CreateWithNullCache()
        {
            Assert.Throws<ArgumentNullException>(() => new CacheConnectorChannelEndpoint(() => new Mock<ILicenseValidationCacheProxy>().Object, null));
        }

        [Test]
        [Description("Checks that an EndPoint returns the correct local proxy value.")]
        public void LocalProxy()
        {
            var proxy = new Mock<ILicenseValidationCacheProxy>();
            var holder = new Mock<ICacheProxyHolder>();
            var endPoint = new CacheConnectorChannelEndpoint(() => proxy.Object, holder.Object);

            Assert.AreSame(proxy.Object, endPoint.LocalProxy());
        }

        [Test]
        [Description("Checks that an EndPoint cannot connect to a proxy with a null AppDomain reference.")]
        public void ConnectWithNullAppDomain()
        {
            var proxy = new Mock<ILicenseValidationCacheProxy>();
            var holder = new Mock<ICacheProxyHolder>();
            var endPoint = new CacheConnectorChannelEndpoint(() => proxy.Object, holder.Object);

            Assert.Throws<ArgumentNullException>(() => endPoint.Connect(null, new Mock<ILicenseValidationCacheProxy>().Object));
        }

        [Test]
        [Description("Checks that an EndPoint cannot connect to a proxy with a null proxy reference.")]
        public void ConnectWithNullProxy()
        {
            var proxy = new Mock<ILicenseValidationCacheProxy>();
            var holder = new Mock<ICacheProxyHolder>();
            var endPoint = new CacheConnectorChannelEndpoint(() => proxy.Object, holder.Object);

            Assert.Throws<ArgumentNullException>(() => endPoint.Connect(AppDomain.CurrentDomain, null));
        }

        [Test]
        [Description("Checks that an EndPoint does not store a proxy if another proxy is already stored for the given AppDomain.")]
        public void ConnectWithExistingAppDomain()
        {
            var proxy = new Mock<ILicenseValidationCacheProxy>();
            ILicenseValidationCacheProxy storedProxy = null;
            var holder = new Mock<ICacheProxyHolder>();
            {
                holder.Setup(h => h.Store(It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<ILicenseValidationCacheProxy>(p => storedProxy = p);
            }

            var endPoint = new CacheConnectorChannelEndpoint(() => proxy.Object, holder.Object);

            endPoint.Connect(AppDomain.CurrentDomain, proxy.Object);
            endPoint.Connect(AppDomain.CurrentDomain, new Mock<ILicenseValidationCacheProxy>().Object);

            Assert.AreSame(proxy.Object, storedProxy);
        }

        [Test]
        [Description("Checks that an EndPoint can be connected to a proxy.")]
        public void Connect()
        {
            var proxy = new Mock<ILicenseValidationCacheProxy>();
            ILicenseValidationCacheProxy storedProxy = null;
            var holder = new Mock<ICacheProxyHolder>();
            {
                holder.Setup(h => h.Store(It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<ILicenseValidationCacheProxy>(p => storedProxy = p);
            }

            var endPoint = new CacheConnectorChannelEndpoint(() => proxy.Object, holder.Object);

            endPoint.Connect(AppDomain.CurrentDomain, proxy.Object);
            Assert.AreSame(proxy.Object, storedProxy);
        }

        [Test]
        [Description("Checks that an EndPoint cannot be disconnected with a null AppDomain reference.")]
        public void DisconnectWithNullAppDomain()
        {
            var proxy = new Mock<ILicenseValidationCacheProxy>();
            var holder = new Mock<ICacheProxyHolder>();
            var endPoint = new CacheConnectorChannelEndpoint(() => proxy.Object, holder.Object);

            Assert.Throws<ArgumentNullException>(() => endPoint.Disconnect(null));
        }

        [Test]
        [Description("Checks that an EndPoint cannot connect to a proxy with a null AppDomain reference.")]
        public void DisconnectWithNonExistingAppDomain()
        {
            var proxy = new Mock<ILicenseValidationCacheProxy>();
            ILicenseValidationCacheProxy storedProxy = null;
            var holder = new Mock<ICacheProxyHolder>();
            {
                holder.Setup(h => h.Store(It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<ILicenseValidationCacheProxy>(p => storedProxy = p);
            }

            var endPoint = new CacheConnectorChannelEndpoint(() => proxy.Object, holder.Object);

            endPoint.Disconnect(AppDomain.CurrentDomain);
            Assert.IsNull(storedProxy);
        }

        [Test]
        [Description("Checks that an EndPoint cannot be disconnected from a proxy.")]
        public void Disconnect()
        {
            var proxy = new Mock<ILicenseValidationCacheProxy>();
            ILicenseValidationCacheProxy storedProxy = null;
            var holder = new Mock<ICacheProxyHolder>();
            {
                holder.Setup(h => h.Store(It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<ILicenseValidationCacheProxy>(p => storedProxy = p);
                holder.Setup(h => h.Release(It.IsAny<ILicenseValidationCacheProxy>()))
                    .Callback<ILicenseValidationCacheProxy>(p => storedProxy = p);
            }

            var endPoint = new CacheConnectorChannelEndpoint(() => proxy.Object, holder.Object);

            endPoint.Connect(AppDomain.CurrentDomain, proxy.Object);
            Assert.AreSame(proxy.Object, storedProxy);

            storedProxy = null;
            endPoint.Disconnect(AppDomain.CurrentDomain);
            Assert.IsNotNull(storedProxy);
        }
    }
}
