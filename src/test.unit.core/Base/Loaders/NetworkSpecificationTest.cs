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
    [Description("Tests the NetworkSpecification class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NetworkSpecificationTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<NetworkSpecification>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<NetworkSpecification> 
                        {
                            new NetworkSpecification("a", "b", true, 5),
                            new NetworkSpecification("a", "c", true, 5),
                            new NetworkSpecification("a", "d", true, 5),
                            new NetworkSpecification("a", "e", true, 5),
                            new NetworkSpecification("a", "f", true, 5),
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<NetworkSpecification>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new NetworkSpecification("a", "b", true, 5),
                        new NetworkSpecification("a", "c", true, 5),
                        new NetworkSpecification("a", "d", true, 5),
                        new NetworkSpecification("a", "e", true, 5),
                        new NetworkSpecification("a", "f", true, 5),
                    },
        };

        [Test]
        [Description("Checks that an object cannot be constructed with a null Name.")]
        public void CreateWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new NetworkSpecification(null, "Mac", true, 5));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with a null MAC address.")]
        public void CreateWithNullMacAddress()
        {
            Assert.Throws<ArgumentNullException>(() => new NetworkSpecification(null, true));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with an empty string as MAC address.")]
        public void CreateWithEmptyMacAddress()
        {
            Assert.Throws<ArgumentException>(() => new NetworkSpecification(string.Empty, true));
        }

        [Test]
        [Description("Checks that an object can be constructed.")]
        public void Create()
        {
            var name = "a";
            var mac = "b";
            bool isConnected = true;
            ulong speed = 5;
            var network = new NetworkSpecification(name, mac, isConnected, speed);

            Assert.AreEqual(name, network.Name);
            Assert.AreEqual(mac, network.MacAddress);
            Assert.IsTrue(network.IsConnected);
            Assert.AreEqual(speed, network.SpeedInBitsPerSecond);
        }
    }
}
