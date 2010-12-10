//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Projects;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the DatasetOfflineInformation class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetOfflineInformationTest
    {
        [Test]
        [Description("Checks that an object cannot be created with a null ID object.")]
        public void CreateWithNullId()
        {
            var reason = new DatasetCreationReason(new DatasetCreationInformation() { CreatedOnRequestOf = DatasetCreator.User });
            var persistence = new Mock<IPersistenceInformation>();

            Assert.Throws<ArgumentNullException>(() => new DatasetOfflineInformation(null, reason, persistence.Object));
        }

        [Test]
        [Description("Checks that an object cannot be created with a null CreationReason object.")]
        public void CreateWithNullCreationReason()
        {
            var id = new DatasetId();
            var persistence = new Mock<IPersistenceInformation>();

            Assert.Throws<ArgumentNullException>(() => new DatasetOfflineInformation(id, null, persistence.Object));
        }

        [Test]
        [Description("Checks that an object cannot be created with a null persistence object.")]
        public void CreateWithNullPersistence()
        {
            var id = new DatasetId();
            var reason = new DatasetCreationReason(new DatasetCreationInformation() { CreatedOnRequestOf = DatasetCreator.User });

            Assert.Throws<ArgumentNullException>(() => new DatasetOfflineInformation(id, reason, null));
        }

        [Test]
        [Description("Checks that an object can be initialized correctly.")]
        public void Create()
        {
            var id = new DatasetId();
            var reason = new DatasetCreationReason(new DatasetCreationInformation() { CreatedOnRequestOf = DatasetCreator.User });
            var persistence = new Mock<IPersistenceInformation>();

            var information = new DatasetOfflineInformation(id, reason, persistence.Object);

            Assert.AreSame(id, information.Id);
            Assert.AreSame(reason, information.ReasonForExistence);
            Assert.AreSame(persistence.Object, information.StoredAt);
        }
    }
}
