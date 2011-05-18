﻿//-----------------------------------------------------------------------
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
    [Description("Tests the NetworkIdentifier class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NetworkIdentifierTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
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
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
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
        [Description("Checks that an object cannot be constructed with a null domain name.")]
        public void CreateWithNullDomainName()
        {
            Assert.Throws<ArgumentNullException>(() => new NetworkIdentifier(null));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with an empty domain name.")]
        public void CreateWithEmptyDomainName()
        {
            Assert.Throws<ArgumentException>(() => new NetworkIdentifier(string.Empty));
        }

        [Test]
        [Description("Checks that an object can be created.")]
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
        [Description("Checks that the network ID can determine if the ID is for the local machine.")]
        public void IsLocalMachine()
        {
            var domain = Environment.MachineName;
            var networkId = new NetworkIdentifier(domain);

            Assert.AreEqual(domain, networkId.DomainName);
            Assert.IsTrue(networkId.IsLocalMachine);
        }

        [Test]
        [Description("Checks that the network ID can determine if the ID belongs to a machine that is part of a machine group.")]
        public void IsPartOfGroup()
        {
            var domain = "a";
            var group = "b";
            Assert.IsFalse(new NetworkIdentifier(domain).IsPartOfGroup);
            Assert.IsTrue(new NetworkIdentifier(domain, group).IsPartOfGroup);
        }
    }
}
