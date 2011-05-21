//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base
{
    [TestFixture]
    [Description("Tests the DatasetOfflineInformation class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetOfflineInformationTest
    {
        [Test]
        [Description("Checks that an object can be initialized correctly.")]
        public void Create()
        {
            var id = new DatasetId();
            var persistence = new Mock<IPersistenceInformation>();
            var reason = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                LoadFrom = persistence.Object,
            };

            var information = new DatasetOfflineInformation(id, reason);

            Assert.AreSame(id, information.Id);
            Assert.AreEqual(reason.CanBeAdopted, information.CanBeAdopted);
            Assert.AreEqual(reason.CanBecomeParent, information.CanBecomeParent);
            Assert.AreEqual(reason.CanBeCopied, information.CanBeCopied);
            Assert.AreEqual(reason.CanBeDeleted, information.CanBeDeleted);
            Assert.AreEqual(reason.CreatedOnRequestOf, information.CreatedBy);
            Assert.AreSame(persistence.Object, information.StoredAt);
        }
    }
}
