//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
    public sealed class MachineTest : EqualityContractVerifierTest
    {
        private sealed class MachineEqualityContractVerifier : EqualityContractVerifier<Machine>
        {
            private readonly Machine m_First = new Machine(new NetworkIdentifier("a"), s_HardwareForLocalMachine);

            private readonly Machine m_Second = new Machine(new NetworkIdentifier("b"), s_HardwareForLocalMachine);

            protected override Machine Copy(Machine original)
            {
                return new Machine(original.Location, original.Specification);
            }

            protected override Machine FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override Machine SecondInstance
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

        private sealed class MachineHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<Machine> m_DistinctInstances
                = new List<Machine> 
                     {
                        new Machine(new NetworkIdentifier("a"), s_HardwareForLocalMachine),
                        new Machine(new NetworkIdentifier("b"), s_HardwareForLocalMachine),
                        new Machine(new NetworkIdentifier("c"), s_HardwareForLocalMachine),
                        new Machine(new NetworkIdentifier("d"), s_HardwareForLocalMachine),
                        new Machine(new NetworkIdentifier("e"), s_HardwareForLocalMachine),
                        new Machine(new NetworkIdentifier("f"), s_HardwareForLocalMachine),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        /// <summary>
        /// Store the hardware specs for the current machine so that we only have to load them once.
        /// </summary>
        private static readonly HardwareSpecification s_HardwareForLocalMachine = HardwareSpecification.ForLocalMachine();

        private readonly MachineHashcodeContractVerfier m_HashcodeVerifier = new MachineHashcodeContractVerfier();

        private readonly MachineEqualityContractVerifier m_EqualityVerifier = new MachineEqualityContractVerifier();

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
            var id = new NetworkIdentifier("a");
            var hardware = s_HardwareForLocalMachine;
            var machine = new Machine(id, hardware);

            Assert.AreEqual(id, machine.Location);
            Assert.AreEqual(hardware, machine.Specification);
        }
    }
}
