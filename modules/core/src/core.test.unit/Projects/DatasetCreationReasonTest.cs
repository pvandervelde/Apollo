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
    [Description("Tests the DatasetCreationReason class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetCreationReasonTest
    {
        [Test]
        [Description("Checks that an object cannot be created with a null information object.")]
        public void CreateWithNullCreationInformation()
        {
            Assert.Throws<ArgumentNullException>(() => new DatasetCreationReason(null));
        }

        [Test]
        [Description("Checks that an object cannot be created with no creator.")]
        public void CreateWithoutCreator()
        {
            var information = new DatasetCreationInformation
            {
                CreatedOnRequestOf = DatasetCreator.None,
            };

            Assert.Throws<CannotCreateDatasetWithoutCreatorException>(() => new DatasetCreationReason(information));
        }

        [Test]
        [Description("Checks that an object can be created correctly.")]
        public void Create()
        {
            var information = new DatasetCreationInformation 
                { 
                    CreatedOnRequestOf = DatasetCreator.User,
                    CanBeAdopted = true,
                    CanBecomeParent = true,
                    CanBeCopied = true,
                    CanBeDeleted = true,
                    LoadFrom = new Mock<IPersistenceInformation>().Object,
                };

            var reason = new DatasetCreationReason(information);

            Assert.AreEqual(information.CreatedOnRequestOf, reason.CreatedBy);
            Assert.AreEqual(information.CanBeAdopted, reason.CanBeAdopted);
            Assert.AreEqual(information.CanBecomeParent, reason.CanBecomeParent);
            Assert.AreEqual(information.CanBeCopied, reason.CanBeCopied);
            Assert.AreEqual(information.CanBeDeleted, reason.CanBeDeleted);
        }
    }
}
