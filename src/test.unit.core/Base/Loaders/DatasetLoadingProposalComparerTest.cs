//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using MbUnit.Framework;

namespace Apollo.Base.Loaders
{
    [TestFixture]
    [Description("Tests the TypeEqualityComparer class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetLoadingProposalComparerTest
    {
        [Test]
        [Description("Checks that the comparison returns the correct value if the first object is null.")]
        public void CompareWithFirstObjectNull()
        {
            var proposal = new DatasetLoadingProposal 
                {
                    Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                    IsAvailable = true,
                    LoadingTime = new TimeSpan(100),
                    TransferTime = new TimeSpan(100),
                    PercentageOfAvailableDisk = 50,
                    PercentageOfMaximumMemory = 50,
                    PercentageOfPhysicalMemory = 50,
                };

            var comparer = new DatasetLoadingProposalComparer();
            Assert.AreEqual(-1, comparer.Compare(null, proposal));
        }

        [Test]
        [Description("Checks that the comparison returns the correct value if the second object is null.")]
        public void CompareWithSecondObjectNull()
        {
            var proposal = new DatasetLoadingProposal
            {
                Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                IsAvailable = true,
                LoadingTime = new TimeSpan(100),
                TransferTime = new TimeSpan(100),
                PercentageOfAvailableDisk = 50,
                PercentageOfMaximumMemory = 50,
                PercentageOfPhysicalMemory = 50,
            };

            var comparer = new DatasetLoadingProposalComparer();
            Assert.AreEqual(1, comparer.Compare(proposal, null));
        }

        [Test]
        [Description("Checks that the comparison throws an exception if the both objects null.")]
        public void CompareWithBothObjectsNull()
        {
            var comparer = new DatasetLoadingProposalComparer();
            Assert.Throws<ArgumentException>(() => comparer.Compare(null, null));
        }

        [Test]
        [Description("Checks that the comparison returns the correct value if both objects are equal.")]
        public void CompareWithEqualObjects()
        {
            var proposal = new DatasetLoadingProposal
            {
                Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                IsAvailable = true,
                LoadingTime = new TimeSpan(100),
                TransferTime = new TimeSpan(100),
                PercentageOfAvailableDisk = 50,
                PercentageOfMaximumMemory = 50,
                PercentageOfPhysicalMemory = 50,
            };

            var comparer = new DatasetLoadingProposalComparer();
            Assert.AreEqual(0, comparer.Compare(proposal, proposal));
        }

        [Test]
        [Description("Checks that the comparison returns the correct value if the objects are not equal.")]
        public void CompareWithUnequalObjects()
        {
            var proposal1 = new DatasetLoadingProposal
            {
                Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                IsAvailable = true,
                LoadingTime = new TimeSpan(50),
                TransferTime = new TimeSpan(50),
                PercentageOfAvailableDisk = 50,
                PercentageOfMaximumMemory = 50,
                PercentageOfPhysicalMemory = 50,
            };

            var proposal2 = new DatasetLoadingProposal 
                {
                    Endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                    IsAvailable = true,
                    LoadingTime = new TimeSpan(500),
                    TransferTime = new TimeSpan(500),
                    PercentageOfAvailableDisk = 50,
                    PercentageOfMaximumMemory = 50,
                    PercentageOfPhysicalMemory = 50,
                };

            var comparer = new DatasetLoadingProposalComparer();
            Assert.AreEqual(-1, comparer.Compare(proposal1, proposal2));
        }
    }
}
