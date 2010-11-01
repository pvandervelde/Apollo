//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.UserInterfaces.Application
{
    [TestFixture]
    [Description("Tests the SystemInformationStorage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SystemInformationStorageTest
    {
        [Test]
        [Description("Checks that the CoreVersion property returns the correct version.")]
        public void CoreVersion()
        {
            var storage = new SystemInformationStorage();
            Assert.AreEqual(typeof(SystemInformationStorage).Assembly.GetName().Version, storage.CoreVersion);
        }
    }
}
