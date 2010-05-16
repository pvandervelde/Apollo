//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Policy;
using Apollo.Utils;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the ApplicationShutdownCapabilityRequestMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class AppDomainSandboxDataTest
    {
        [Test]
        [Description("Checks that an object cannot be created with a null collection.")]
        public void CreateWithNullFullTrustAssemblies()
        {
            Assert.Throws<ArgumentNullException>(() => new AppDomainSandboxData(SecurityLevel.Kernel, null));
        }

        [Test]
        [Description("Checks that an AppDomainSandboxData object can be created.")]
        public void Create()
        {
            var strongName = Assembly.GetExecutingAssembly().GetStrongName();
            var data = new AppDomainSandboxData(SecurityLevel.Kernel, new List<StrongName> { strongName });

            Assert.AreEqual(SecurityLevel.Kernel, data.Level);
            Assert.AreElementsEqualIgnoringOrder(new List<StrongName> { strongName }, data.FullTrustAssemblies);
        }
    }
}
