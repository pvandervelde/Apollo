//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;
using Nuclei.Communication;
using Nuclei.Diagnostics;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MachineSelectorParameterTest
    {
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

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<Parameter>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
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
                    CreateInstance(10),
                },
        };

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

            Assert.AreElementsSame(
                new List<DistributionSuggestion>
                    {
                        suggestion,
                    }, 
                parameter.Suggestions);
        }
    }
}
