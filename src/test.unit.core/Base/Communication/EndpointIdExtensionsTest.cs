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
    [Description("Tests the EndpointIdExtensions class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointIdExtensionsTest
    {
        [Test]
        [Description("Checks that the deserialization of an endpoint ID is done correctly.")]
        public void Deserialize()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var text = id.ToString();

            var otherId = EndpointIdExtensions.Deserialize(text);
            Assert.AreEqual(id, otherId);
        }

        [Test]
        [Description("Checks that an endpoint ID cannot be deserialized from an empty string.")]
        public void DeserializeWithEmptyString()
        {
            Assert.Throws<ArgumentException>(() => EndpointIdExtensions.Deserialize(string.Empty));
        }

        [Test]
        [Description("Checks that it is possible to determine from which machine an endpoint comes.")]
        public void IsOnMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            Assert.IsTrue(id.IsOnMachine(Environment.MachineName));
            Assert.IsFalse(id.IsOnMachine("foobar"));
        }

        [Test]
        [Description("Checks that it is possible to determine if an endpoint comes from the local machine.")]
        public void IsOnLocalMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            Assert.IsTrue(id.IsOnLocalMachine()); 
        }

        [Test]
        [Description("Checks that it is possible to extract the machine name from an endpoint ID.")]
        public void OriginatesOnMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var machineName = id.OriginatesOnMachine();
            Assert.AreEqual(Environment.MachineName, machineName);
        }
    }
}
