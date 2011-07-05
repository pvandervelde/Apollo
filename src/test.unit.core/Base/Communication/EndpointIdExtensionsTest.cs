﻿//-----------------------------------------------------------------------
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
    public sealed class EndpointIdExtensionsTest
    {
        [Test]
        public void Deserialize()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var text = id.ToString();

            var otherId = EndpointIdExtensions.Deserialize(text);
            Assert.AreEqual(id, otherId);
        }

        [Test]
        public void DeserializeWithEmptyString()
        {
            Assert.Throws<ArgumentException>(() => EndpointIdExtensions.Deserialize(string.Empty));
        }

        [Test]
        public void IsOnMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            Assert.IsTrue(id.IsOnMachine(Environment.MachineName));
            Assert.IsFalse(id.IsOnMachine("foobar"));
        }

        [Test]
        public void IsOnLocalMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            Assert.IsTrue(id.IsOnLocalMachine()); 
        }

        [Test]
        public void OriginatesOnMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var machineName = id.OriginatesOnMachine();
            Assert.AreEqual(Environment.MachineName, machineName);
        }
    }
}
