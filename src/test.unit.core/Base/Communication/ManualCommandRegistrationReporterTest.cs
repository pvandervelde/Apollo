//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ManualCommandRegistrationReporterTest
    {
        [Test]
        public void RecentlyRegisteredCommand()
        {
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var type = CommandSetProxyExtensions.FromType(typeof(IDatasetApplicationCommands));
            var hasFired = false;

            var reporter = new ManualCommandRegistrationReporter();
            reporter.OnNewCommandRegistered += 
                (s, e) =>
                {
                    hasFired = true;
                    Assert.AreSame(endpoint, e.Endpoint);
                    Assert.AreSame(type, e.Command);
                };

            reporter.RecentlyRegisteredCommand(endpoint, type);
            Assert.IsTrue(hasFired);
        }
    }
}
