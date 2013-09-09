//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Utilities;
using Moq;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MachineSelectorPresenterTest
    {
        private static DistributionPlan CreateNewDistributionPlan(
           DatasetActivationProposal proposal,
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
        public void Initialize()
        {
            var context = new Mock<IContextAware>();

            var view = new Mock<IMachineSelectorView>();
            {
                view.SetupSet(v => v.Model = It.IsAny<MachineSelectorModel>())
                    .Verifiable();
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var plan = CreateNewDistributionPlan(
                new DatasetActivationProposal(),
                CreateOfflineInfo(new Mock<IPersistenceInformation>().Object),
                systemDiagnostics);
            var suggestion = new DistributionSuggestion(plan);

            var parameter = new MachineSelectorParameter(
                context.Object,
                new List<DistributionSuggestion>
                    {
                        suggestion,
                    });

            var container = new Mock<IDependencyInjectionProxy>();
            {
                container.Setup(c => c.Resolve<IContextAware>())
                    .Returns(context.Object);
            }

            var presenter = new MachineSelectorPresenter(container.Object);
            ((IPresenter)presenter).Initialize(view.Object, parameter);

            Assert.AreSame(view.Object, presenter.View);
            Assert.AreSame(parameter, presenter.Parameter);
            view.VerifySet(v => v.Model = It.IsAny<MachineSelectorModel>(), Times.Once());
        }
    }
}
