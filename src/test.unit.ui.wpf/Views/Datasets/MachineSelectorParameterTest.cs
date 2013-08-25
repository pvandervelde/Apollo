//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using Moq;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MachineSelectorParameterTest : EqualityContractVerifierTest
    {
        private sealed class MachineSelectorParameterEqualityContractVerifier : EqualityContractVerifier<MachineSelectorParameter>
        {
            private readonly MachineSelectorParameter m_First = CreateInstance(1);

            private readonly MachineSelectorParameter m_Second = CreateInstance(2);

            protected override MachineSelectorParameter Copy(MachineSelectorParameter original)
            {
                return new MachineSelectorParameter(new Mock<IContextAware>().Object, original.Suggestions);
            }

            protected override MachineSelectorParameter FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override MachineSelectorParameter SecondInstance
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
                    return false;
                }
            }
        }

        private sealed class MachineSelectorParameterHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<MachineSelectorParameter> m_DistinctInstances
                = new List<MachineSelectorParameter> 
                     {
                        CreateInstance(1),
                        CreateInstance(2),
                        CreateInstance(3),
                        CreateInstance(4),
                        CreateInstance(5),
                        CreateInstance(6),
                        CreateInstance(7),
                        CreateInstance(8),
                        CreateInstance(9),
                        CreateInstance(10)
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private static DistributionPlan CreateNewDistributionPlan(
           DatasetLoadingProposal proposal,
            IDatasetOfflineInformation offlineInfo,
            SystemDiagnostics systemDiagnostics)
        {
            var plan = new DistributionPlan(
                (p, t, r) => Task<DatasetOnlineInformation>.Factory.StartNew(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                        new Mock<INotifyOfRemoteEndpointEvents>().Object,
                        systemDiagnostics),
                    t),
                    offlineInfo,
                NetworkIdentifier.ForLocalMachine(),
                proposal);
            return plan;
        }

        private static IDatasetOfflineInformation CreateOfflineInfo(IPersistenceInformation storage)
        {
            var mock = new Mock<IDatasetOfflineInformation>();
            {
                mock.Setup(d => d.CanBeAdopted)
                    .Returns(false);
                mock.Setup(d => d.CanBecomeParent)
                    .Returns(true);
                mock.Setup(d => d.CanBeCopied)
                    .Returns(false);
                mock.Setup(d => d.CanBeDeleted)
                    .Returns(true);
                mock.Setup(d => d.CreatedBy)
                    .Returns(DatasetCreator.User);
                mock.Setup(d => d.StoredAt)
                    .Returns(storage);
            }

            return mock.Object;
        }

        private static MachineSelectorParameter CreateInstance(int rating)
        {
            var context = new Mock<IContextAware>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var proposal = new DatasetLoadingProposal
            {
                PercentageOfAvailableDisk = 5 * rating,
                PercentageOfMaximumMemory = 75,
                PercentageOfPhysicalMemory = 90,
                TransferTime = new TimeSpan(0, 0, rating, 0, 0),
                LoadingTime = new TimeSpan(0, 0, 0, rating, 0),
            };
            var plan = CreateNewDistributionPlan(
                proposal,
                CreateOfflineInfo(new Mock<IPersistenceInformation>().Object),
                systemDiagnostics);
            var suggestion = new DistributionSuggestion(plan);

            return new MachineSelectorParameter(
                context.Object,
                new List<DistributionSuggestion>
                    {
                        suggestion,
                    });
        }

        private readonly MachineSelectorParameterHashcodeContractVerfier m_HashcodeVerifier 
            = new MachineSelectorParameterHashcodeContractVerfier();

        private readonly MachineSelectorParameterEqualityContractVerifier m_EqualityVerifier 
            = new MachineSelectorParameterEqualityContractVerifier();

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
            var context = new Mock<IContextAware>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var proposal = new DatasetLoadingProposal
            {
                PercentageOfAvailableDisk = 5,
                PercentageOfMaximumMemory = 100,
                PercentageOfPhysicalMemory = 75,
                TransferTime = new TimeSpan(0, 0, 1, 0, 0),
                LoadingTime = new TimeSpan(0, 0, 0, 1, 0),
            };
            var plan = CreateNewDistributionPlan(
                proposal,
                CreateOfflineInfo(new Mock<IPersistenceInformation>().Object),
                systemDiagnostics);
            var suggestion = new DistributionSuggestion(plan);

            var parameter = new MachineSelectorParameter(
                context.Object,
                new List<DistributionSuggestion>
                    {
                        suggestion,
                    });

            Assert.That(
                parameter.Suggestions,
                Is.EquivalentTo(
                    new List<DistributionSuggestion>
                    {
                        suggestion,
                    }));
        }
    }
}
