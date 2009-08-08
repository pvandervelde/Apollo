//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Apollo.Utils.Fusion;
using MbUnit.Framework;

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

        private void InitializeFusionHelper(FusionHelper helper)
        {
            // Can effectively just return the current assembly / gallio assemblies / system
            helper.FileEnumerator = () => { return m_Assemblies.Keys.ToArray<string>(); };
            helper.AssemblyLoader = (assemblyPath) =>
            {
                return m_Assemblies[assemblyPath];
            };
        }

        private string CreateFullAssemblyName(string assemblyName, Version version, CultureInfo culture, string publicToken)
        {
            return string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", assemblyName, version, culture, publicToken);
        }

        // Private method used to run the FusionHelper.LoadAssembly method
        private Assembly ExecuteLoadAssembly(FusionHelper helper, string assemblyName)
        {
            return helper.LocateAssemblyOnAssemblyLoadFailure(null, new ResolveEventArgs(assemblyName));
        }

        [Test]
        public void LoadAssemblyWithExistingSimpleName()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var name = Assembly.GetExecutingAssembly().GetName().Name;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame<Assembly>(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingSimpleName()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var name = typeof(FusionHelper).Assembly.GetName().Name;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithExistingSimpleNameWithExtension()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var name = Assembly.GetExecutingAssembly().GetName().Name + ".dll";
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame<Assembly>(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingSimpleNameWithExtension()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var name = typeof(FusionHelper).Assembly.GetName().Name + ".dll";
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithExistingFullName()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var name = Assembly.GetExecutingAssembly().GetName().FullName;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame<Assembly>(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnModule()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var assemblyName = typeof(FusionHelper).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, assemblyName.Version, assemblyName.CultureInfo, "null");
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnVersion()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, new Version(0, 0, 0, 0), assemblyName.CultureInfo, new System.Text.ASCIIEncoding().GetString(assemblyName.GetPublicKeyToken()));
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }


        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnCulture()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, assemblyName.Version, new CultureInfo("en-US"), new System.Text.ASCIIEncoding().GetString(assemblyName.GetPublicKeyToken()));
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnPublicToken()
        {
            var helper = new FusionHelper();
            InitializeFusionHelper(helper);

            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, assemblyName.Version, new CultureInfo("en-US"), "null");
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }
    }
}
