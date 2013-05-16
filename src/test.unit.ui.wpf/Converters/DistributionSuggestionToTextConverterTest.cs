//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;
using Nuclei.Communication;
using Nuclei.Diagnostics;

namespace Apollo.UI.Wpf.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DistributionSuggestionToTextConverterTest
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

        [Test]
        public void ConvertWithDistributionSuggestion()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var plan = CreateNewDistributionPlan(
                new DatasetLoadingProposal(),
                CreateOfflineInfo(new Mock<IPersistenceInformation>().Object),
                systemDiagnostics);

            var model = new DistributionSuggestion(plan);

            var converter = new DistributionSuggestionToTextConverter();
            var convertedValue = converter.Convert(model, null, null, null);
            Assert.AreEqual(plan.MachineToDistributeTo.ToString(), convertedValue);
        }

        [Test]
        public void ConvertWithNullReference()
        {
            var converter = new DistributionSuggestionToTextConverter();
            var convertedValue = converter.Convert(null, null, null, null);
            Assert.AreEqual("Unknown network resource ...", convertedValue);
        }

        [Test]
        public void ConvertWithNonDistributionSuggestionObject()
        {
            var converter = new DistributionSuggestionToTextConverter();
            var convertedValue = converter.Convert(new object(), null, null, null);
            Assert.AreEqual("Unknown network resource ...", convertedValue);
        }
    }
}
