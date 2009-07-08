//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using System.Reflection;
using Gallio.Reflection;
using Apollo.Utils.Fusion;
using System.IO;
using System.Globalization;

namespace Apollo.Utils
{
    [TestFixture]
    public sealed class FusionHelperTest
    {
        private readonly Dictionary<string, Assembly> m_Assemblies = new Dictionary<string, Assembly>();

        private string GetAssemblyPath(Assembly assembly)
        {
            var codebase = assembly.CodeBase;
            var uri = new Uri(codebase);
            return uri.LocalPath;
        }

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            // mscorlib
            m_Assemblies.Add(GetAssemblyPath(typeof(string).Assembly), typeof(string).Assembly);
            // gallio
            m_Assemblies.Add(GetAssemblyPath(typeof(FixtureSetUpAttribute).Assembly), typeof(FixtureSetUpAttribute).Assembly);
            // us
            m_Assemblies.Add(GetAssemblyPath(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly());
        }

        [TearDown]
        public void TearDown()
        {
            FusionHelper.FileEnumerator = null;
            FusionHelper.AssemblyLoader = null;

            FusionHelper.RemoveAllProbingDirectories();
        }

        private void InitializeFusionHelper()
        {
            // Create the probing directories. There just needs to be one.
            FusionHelper.AddProbingDirectory(new DirectoryInfo(Path.GetDirectoryName(GetAssemblyPath(Assembly.GetExecutingAssembly()))));

            // Can effectively just return the current assembly / gallio assemblies / system
            FusionHelper.FileEnumerator = (basePath) => { return m_Assemblies.Keys.ToArray<string>(); };
            FusionHelper.AssemblyLoader = (assemblyPath) =>
            {
                return m_Assemblies[assemblyPath];
            };
        }

        private string CreateFullAssemblyName(string assemblyName, Version version, CultureInfo culture, string publicToken)
        {
            return string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", assemblyName, version, culture, publicToken);
        }

        // Private method used to run the FusionHelper.LoadAssembly method
        private Assembly ExecuteLoadAssembly(string assemblyName)
        {
            return FusionHelper.LocateAssemblyOnAssemblyLoadFailure(null, new ResolveEventArgs(assemblyName));
        }

        [Test]
        public void LoadAssemblyWithoutProbingDirectories()
        { 
            Assert.IsNull(ExecuteLoadAssembly(Assembly.GetExecutingAssembly().GetName().Name));
        }

        [Test]
        public void LoadAssemblyWithExistingSimpleName()
        {
            InitializeFusionHelper();

            var name = Assembly.GetExecutingAssembly().GetName().Name;
            var result = ExecuteLoadAssembly(name);
            Assert.AreSame<Assembly>(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingSimpleName()
        {
            InitializeFusionHelper();

            var name = typeof(FusionHelper).Assembly.GetName().Name;
            var result = ExecuteLoadAssembly(name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithExistingSimpleNameWithExtension()
        {
            InitializeFusionHelper();

            var name = Assembly.GetExecutingAssembly().GetName().Name + ".dll";
            var result = ExecuteLoadAssembly(name);
            Assert.AreSame<Assembly>(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingSimpleNameWithExtension()
        {
            InitializeFusionHelper();

            var name = typeof(FusionHelper).Assembly.GetName().Name + ".dll";
            var result = ExecuteLoadAssembly(name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithExistingFullName()
        {
            InitializeFusionHelper();

            var name = Assembly.GetExecutingAssembly().GetName().FullName;
            var result = ExecuteLoadAssembly(name);
            Assert.AreSame<Assembly>(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnModule()
        {
            InitializeFusionHelper();

            var assemblyName = typeof(FusionHelper).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, assemblyName.Version, assemblyName.CultureInfo, "null");
            var result = ExecuteLoadAssembly(name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnVersion()
        {
            InitializeFusionHelper();

            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, new Version(0, 0, 0, 0), assemblyName.CultureInfo, new System.Text.ASCIIEncoding().GetString(assemblyName.GetPublicKeyToken()));
            var result = ExecuteLoadAssembly(name);
            Assert.IsNull(result);
        }


        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnCulture()
        {
            InitializeFusionHelper();

            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, assemblyName.Version, new CultureInfo("en-US"), new System.Text.ASCIIEncoding().GetString(assemblyName.GetPublicKeyToken()));
            var result = ExecuteLoadAssembly(name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnPublicToken()
        {
            InitializeFusionHelper();

            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, assemblyName.Version, new CultureInfo("en-US"), "null");
            var result = ExecuteLoadAssembly(name);
            Assert.IsNull(result);
        }
    }
}
