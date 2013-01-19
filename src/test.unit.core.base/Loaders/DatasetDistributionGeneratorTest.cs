//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;
using Utilities.Communication;
using Utilities.Diagnostics;

namespace Apollo.Core.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetDistributionGeneratorTest
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
                    t,
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler()),
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

        [Test]
        public void ProposeDistributionFor()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var first = new Mock<IGenerateDistributionProposals>();
            {
                first.Setup(p => p.ProposeDistributionFor(It.IsAny<DatasetRequest>(), It.IsAny<CancellationToken>()))
                    .Returns(
                        new DistributionPlan[] 
                        {
                            CreateNewDistributionPlan(
                                new DatasetLoadingProposal(),
                                CreateOfflineInfo(new Mock<IPersistenceInformation>().Object),
                                systemDiagnostics)
                        })
                        .Verifiable();
            }

            var second = new Mock<IGenerateDistributionProposals>();
            {
                second.Setup(p => p.ProposeDistributionFor(It.IsAny<DatasetRequest>(), It.IsAny<CancellationToken>()))
                    .Returns(
                        new DistributionPlan[] 
                        {
                            CreateNewDistributionPlan(
                                new DatasetLoadingProposal(),
                                CreateOfflineInfo(new Mock<IPersistenceInformation>().Object),
                                systemDiagnostics)
                        })
                        .Verifiable();
            }

            var generator = new DatasetDistributionGenerator(new IGenerateDistributionProposals[] { first.Object, second.Object });
            var results = generator.ProposeDistributionFor(new DatasetRequest(), new CancellationToken());
            
            Assert.AreEqual(2, results.Count());
            first.Verify(f => f.ProposeDistributionFor(It.IsAny<DatasetRequest>(), It.IsAny<CancellationToken>()), Times.Once());
            second.Verify(s => s.ProposeDistributionFor(It.IsAny<DatasetRequest>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
