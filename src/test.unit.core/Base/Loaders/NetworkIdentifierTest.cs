//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Loaders;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NetworkIdentifierTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<NetworkIdentifier>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<NetworkIdentifier> 
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
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<NetworkIdentifier>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
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
                    },
        };

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
