//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.UI.Common.Views.Projects
{
    [TestFixture]
    [Description("Tests the MissingNotificationActionException class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectModelTest
    {
        [Test]
        [Description("Checks that the model cannot be created without a facade.")]
        public void CreateWithNullFacade()
        {
            Assert.Throws<ArgumentNullException>(() => new ProjectModel(null));
        }

        [Test]
        [Description("Checks that the model can be created.")]
        [Ignore("Not implemented yet")]
        public void Create()
        {
        }
    }
}
