using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using System.Reflection;
using Gallio.Reflection;
using Apollo.Utils.Fusion;

namespace Apollo.Utils
{
    [TestFixture]
    public sealed class FusionHelperTest
    {
        [FixtureTearDown]
        public void FixtureTearDown()
        {
            FusionHelper.FileEnumerator = null;
            FusionHelper.AssemblyLoader = null;

            FusionHelper.RemoveAllProbingDirectories();
        }

        // Private method used to run the FusionHelper.LoadAssembly method
        private Assembly ExecuteLoadAssembly(string path)
        {
            return FusionHelper.LocateAssemblyOnAssemblyLoadFailure(null, new ResolveEventArgs(path));
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithExistingSimpleName()
        { }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithNonExistingSimpleName()
        { }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithExistingSimpleNameWithExtension()
        { }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithNonExistingSimpleNameWithExtension()
        { }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithExistingFullName()
        { }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithNonExistingFullNameBasedOnModule()
        { }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithNonExistingFullNameBasedOnVersion()
        { }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithNonExistingFullNameBasedOnCulture()
        { }

        [Test]
        [Ignore("Not implemented yet")]
        public void LoadAssemblyWithNonExistingFullNameBasedOnPublicToken()
        { }
    }
}
