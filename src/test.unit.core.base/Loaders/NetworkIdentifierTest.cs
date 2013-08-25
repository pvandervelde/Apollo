//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Core.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NetworkIdentifierTest : EqualityContractVerifierTest
    {
        private sealed class NetworkIdentifierEqualityContractVerifier : EqualityContractVerifier<NetworkIdentifier>
        {
            private readonly NetworkIdentifier m_First = new NetworkIdentifier("a", "b");

            private readonly NetworkIdentifier m_Second = new NetworkIdentifier("b", "b");

            protected override NetworkIdentifier Copy(NetworkIdentifier original)
            {
                return new NetworkIdentifier(original.DomainName, original.Group);
            }

            protected override NetworkIdentifier FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override NetworkIdentifier SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class NetworkIdentifierHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<NetworkIdentifier> m_DistinctInstances
                = new List<NetworkIdentifier> 
                     {
                        new NetworkIdentifier("a", "b"),
                        new NetworkIdentifier("b", "b"),
                        new NetworkIdentifier("c", "b"),
                        new NetworkIdentifier("d", "b"),
                        new NetworkIdentifier("e", "b"),
                        new NetworkIdentifier("a", "c"),
                        new NetworkIdentifier("a", "d"),
                        new NetworkIdentifier("a", "e"),
                        new NetworkIdentifier("a", "f"),
                        new NetworkIdentifier("a", "g"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly NetworkIdentifierHashcodeContractVerfier m_HashcodeVerifier = new NetworkIdentifierHashcodeContractVerfier();

        private readonly NetworkIdentifierEqualityContractVerifier m_EqualityVerifier = new NetworkIdentifierEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }

        [Test]
        public void Create()
        {
            var domain = "a";
            var group = "b";
            var networkId = new NetworkIdentifier(domain, group);

            Assert.AreEqual(domain, networkId.DomainName);
            Assert.AreEqual(group, networkId.Group);
            Assert.IsTrue(networkId.IsPartOfGroup);
            Assert.IsFalse(networkId.IsLocalMachine);
        }

        [Test]
        public void IsLocalMachine()
        {
            var domain = Environment.MachineName;
            var networkId = new NetworkIdentifier(domain);

            Assert.AreEqual(domain, networkId.DomainName);
            Assert.IsTrue(networkId.IsLocalMachine);
        }

        [Test]
        public void IsPartOfGroup()
        {
            var domain = "a";
            var group = "b";
            Assert.IsFalse(new NetworkIdentifier(domain).IsPartOfGroup);
            Assert.IsTrue(new NetworkIdentifier(domain, group).IsPartOfGroup);
        }
    }
}
