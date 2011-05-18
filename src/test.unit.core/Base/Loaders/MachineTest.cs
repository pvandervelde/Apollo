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
    [Description("Tests the ProcessorSpecification class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class MachineTest
    {
        /// <summary>
        /// Store the hardware specs for the current machine so that we only have to load them once.
        /// </summary>
        private static readonly HardwareSpecification m_HardwareForLocalMachine = HardwareSpecification.ForLocalMachine();

        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<Machine>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<Machine> 
                        {
                            new Machine(new NetworkIdentifier("a"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                            new Machine(new NetworkIdentifier("b"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                            new Machine(new NetworkIdentifier("c"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                            new Machine(new NetworkIdentifier("d"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                            new Machine(new NetworkIdentifier("e"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                            new Machine(new NetworkIdentifier("f"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<Machine>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new Machine(new NetworkIdentifier("a"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                        new Machine(new NetworkIdentifier("b"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                        new Machine(new NetworkIdentifier("c"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                        new Machine(new NetworkIdentifier("d"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                        new Machine(new NetworkIdentifier("e"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                        new Machine(new NetworkIdentifier("f"), m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()),
                    },
        };

        [Test]
        [Description("Checks that an object cannot be constructed with a null network identifier.")]
        public void CreateWithNullIdentifier()
        {
            Assert.Throws<ArgumentNullException>(() => new Machine(null, m_HardwareForLocalMachine, new Dictionary<BaselineId, BaselineResult>()));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with a null hardware specification.")]
        public void CreateWithNullHardwareSpecification()
        {
            Assert.Throws<ArgumentNullException>(() => new Machine(new NetworkIdentifier("a"), null, new Dictionary<BaselineId, BaselineResult>()));
        }

        [Test]
        [Description("Checks that an object cannot be constructed with a null collection of baselines.")]
        public void CreateWithNullBaseLineCollection()
        {
            Assert.Throws<ArgumentNullException>(() => new Machine(new NetworkIdentifier("a"), m_HardwareForLocalMachine, null));
        }

        [Test]
        [Description("Checks that an object can be constructed.")]
        public void Create()
        {
            var id = new NetworkIdentifier("a");
            var hardware = m_HardwareForLocalMachine;
            var baselines = new Dictionary<BaselineId, BaselineResult>();
            var machine = new Machine(id, hardware, baselines);

            Assert.AreEqual(id, machine.Location);
            Assert.AreEqual(hardware, machine.Specification);
        }
    }
}
