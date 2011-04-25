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
    [Description("Tests the CoreProxy class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CoreProxyTest
    {
        [Test]
        [Description("Checks that the object returns the correct names for services to which it should be connected.")]
        public void ServicesToConnectTo()
        {
            var service = new CoreProxy(new Mock<IKernel>().Object);
            Assert.IsFalse(service.ServicesToConnectTo().Exists());
        }

        [Test]
        [Description("Checks that ShutdownRequestMessage is handled correctly when the kernel is unable to stop yet it is forced too.")]
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
        [Description("Checks that ApplicationShutdownCapabilityRequestMessage is handled correctly.")]
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
