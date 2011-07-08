//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CoreProxyTest
    {
        [Test]
        public void ServicesToConnectTo()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object);
            Assert.IsFalse(service.ServicesToConnectTo().Exists());
        }

        [Test]
        public void Shutdown()
        {
            bool wasShutdown = false;
            var kernel = new Mock<IKernel>();
            {
                kernel.Setup(k => k.Shutdown())
                    .Callback(() => wasShutdown = true);
            }

            var service = new CoreProxy(kernel.Object);

            service.Start();
            var task = service.Shutdown();
            task.Wait();

            Assert.IsTrue(wasShutdown);
        }

        [Test]
        public void NotifyServicesOfStartupCompletion()
        {
            var kernel = new Mock<IKernel>();
            var service = new CoreProxy(kernel.Object);

            bool wasInvoked = false;
            service.OnStartupComplete += (s, e) => { wasInvoked = true; };

            service.Start();
            service.NotifyServicesOfStartupCompletion();

            Assert.IsTrue(wasInvoked);
        }
    }
}
