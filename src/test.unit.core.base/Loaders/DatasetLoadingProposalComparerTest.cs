//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication;
using NUnit.Framework;

namespace Apollo.Core.Base.Activation
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetLoadingProposalComparerTest
    {
        [Test]
        public void CompareWithFirstObjectNull()
        {
            var proposal = new DatasetActivationProposal 
                {
                    Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                    IsAvailable = true,
                    ActivationTime = new TimeSpan(100),
                    TransferTime = new TimeSpan(100),
                    PercentageOfAvailableDisk = 50,
                    PercentageOfMaximumMemory = 50,
                    PercentageOfPhysicalMemory = 50,
                };

            var comparer = new DatasetActivationProposalComparer();
            Assert.AreEqual(-1, comparer.Compare(null, proposal));
        }

        [Test]
        public void CompareWithSecondObjectNull()
        {
            var proposal = new DatasetActivationProposal
            {
                Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                IsAvailable = true,
                ActivationTime = new TimeSpan(100),
                TransferTime = new TimeSpan(100),
                PercentageOfAvailableDisk = 50,
                PercentageOfMaximumMemory = 50,
                PercentageOfPhysicalMemory = 50,
            };

            var comparer = new DatasetActivationProposalComparer();
            Assert.AreEqual(1, comparer.Compare(proposal, null));
        }

        [Test]
        public void CompareWithBothObjectsNull()
        {
            var comparer = new DatasetActivationProposalComparer();
            Assert.Throws<ArgumentNullException>(() => comparer.Compare(null, null));
        }

        [Test]
        public void CompareWithEqualObjects()
        {
            var proposal = new DatasetActivationProposal
            {
                Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                IsAvailable = true,
                ActivationTime = new TimeSpan(100),
                TransferTime = new TimeSpan(100),
                PercentageOfAvailableDisk = 50,
                PercentageOfMaximumMemory = 50,
                PercentageOfPhysicalMemory = 50,
            };

            var comparer = new DatasetActivationProposalComparer();
            Assert.AreEqual(0, comparer.Compare(proposal, proposal));
        }

        [Test]
        public void CompareWithUnequalObjects()
        {
            var proposal1 = new DatasetActivationProposal
            {
                Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                IsAvailable = true,
                ActivationTime = new TimeSpan(50),
                TransferTime = new TimeSpan(50),
                PercentageOfAvailableDisk = 50,
                PercentageOfMaximumMemory = 50,
                PercentageOfPhysicalMemory = 50,
            };

            var proposal2 = new DatasetActivationProposal 
                {
                    Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                    IsAvailable = true,
                    ActivationTime = new TimeSpan(500),
                    TransferTime = new TimeSpan(500),
                    PercentageOfAvailableDisk = 50,
                    PercentageOfMaximumMemory = 50,
                    PercentageOfPhysicalMemory = 50,
                };

            var comparer = new DatasetActivationProposalComparer();
            Assert.AreEqual(-1, comparer.Compare(proposal1, proposal2));
        }
    }
}
