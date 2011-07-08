//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Loaders
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetDistributionGeneratorTest
    {
        [Test]
        public void ProposeDistributionFor()
        {
            Action<LogSeverityProxy, string> logger = (p, s) => { };

            var first = new Mock<IGenerateDistributionProposals>();
            {
                first.Setup(p => p.ProposeDistributionFor(It.IsAny<DatasetRequest>(), It.IsAny<CancellationToken>()))
                    .Returns(
                        new DistributionPlan[] 
                        {
                            new DistributionPlan(
                                (p, t) => Task<DatasetOnlineInformation>.Factory.StartNew(
                                    () => new DatasetOnlineInformation(
                                        new DatasetId(),
                                        new EndpointId("id"),
                                        new NetworkIdentifier("machine"),
                                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                                        logger),
                                    t,
                                    TaskCreationOptions.None,
                                    new CurrentThreadTaskScheduler()),
                                new DatasetOfflineInformation(
                                    new DatasetId(),
                                    new DatasetCreationInformation()
                                    {
                                        CreatedOnRequestOf = DatasetCreator.User,
                                        CanBecomeParent = true,
                                        CanBeAdopted = false,
                                        CanBeCopied = false,
                                        CanBeDeleted = true,
                                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                                    }),
                                new NetworkIdentifier("mymachine"),
                                new DatasetLoadingProposal())
                        })
                        .Verifiable();
            }

            var second = new Mock<IGenerateDistributionProposals>();
            {
                second.Setup(p => p.ProposeDistributionFor(It.IsAny<DatasetRequest>(), It.IsAny<CancellationToken>()))
                    .Returns(
                        new DistributionPlan[] 
                        {
                            new DistributionPlan(
                                (p, t) => Task<DatasetOnlineInformation>.Factory.StartNew(
                                    () => new DatasetOnlineInformation(
                                        new DatasetId(),
                                        new EndpointId("id"),
                                        new NetworkIdentifier("machine"),
                                        new Mock<ISendCommandsToRemoteEndpoints>().Object,
                                        logger),
                                    t,
                                    TaskCreationOptions.None,
                                    new CurrentThreadTaskScheduler()),
                                new DatasetOfflineInformation(
                                    new DatasetId(),
                                    new DatasetCreationInformation()
                                    {
                                        CreatedOnRequestOf = DatasetCreator.User,
                                        CanBecomeParent = true,
                                        CanBeAdopted = false,
                                        CanBeCopied = false,
                                        CanBeDeleted = true,
                                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                                    }),
                                new NetworkIdentifier("mymachine"),
                                new DatasetLoadingProposal())
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
