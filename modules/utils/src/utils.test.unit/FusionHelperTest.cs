//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Apollo.Utils.Fusion;
using MbUnit.Framework;
using Moq;

namespace Apollo.Utils
{
    [TestFixture]
    [Description("Tests the FusionHelper class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class FusionHelperTest
    {
        private readonly Dictionary<string, Assembly> m_Assemblies = new Dictionary<string, Assembly>();

        private static string GetAssemblyPath(Assembly assembly)
        {
            var codebase = assembly.CodeBase;
            var uri = new Uri(codebase);
            return uri.LocalPath;
        }

        private static string CreateFullAssemblyName(string assemblyName, Version version, CultureInfo culture, string publicToken)
        {
            return string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", assemblyName, version, culture, publicToken);
        }

        // Private method used to run the FusionHelper.LoadAssembly method
        private static Assembly ExecuteLoadAssembly(FusionHelper helper, string assemblyName)
        {
            return helper.LocateAssemblyOnAssemblyLoadFailure(null, new ResolveEventArgs(assemblyName));
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

        private FusionHelper InitializeFusionHelper()
        {
            var mockFileConstants = new Mock<IFileConstants>();
            mockFileConstants.Setup(constants => constants.AssemblyExtension)
                .Returns(".dll");

            // Can effectively just return the current assembly / gallio assemblies / system
            var helper = new FusionHelper(() => m_Assemblies.Keys.ToArray<string>(), mockFileConstants.Object);
            helper.AssemblyLoader = (assemblyPath) =>
            {
                return m_Assemblies[assemblyPath];
            };

            return helper;
        }

        [Test]
        [Description("Tests that an assembly can be loaded with a simple name, i.e. only the module name.")]
        public void LoadAssemblyWithExistingSimpleName()
        {
            var helper = InitializeFusionHelper();
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        [Description("Tests that an assembly cannot be loaded with a non-existing simple name, i.e. only the module name.")]
        public void LoadAssemblyWithNonExistingSimpleName()
        {
            var helper = InitializeFusionHelper();
            var name = typeof(FusionHelper).Assembly.GetName().Name;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        [Description("Tests that an assembly can be loaded with a simple name with file extension.")]
        public void LoadAssemblyWithExistingSimpleNameWithExtension()
        {
            var helper = InitializeFusionHelper();
            var name = Assembly.GetExecutingAssembly().GetName().Name + ".dll";
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        [Description("Tests that an assembly cannot be loaded with a non-existing simple name with file extension.")]
        public void LoadAssemblyWithNonExistingSimpleNameWithExtension()
        {
            var helper = InitializeFusionHelper();
            var name = typeof(FusionHelper).Assembly.GetName().Name + ".dll";
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        [Description("Tests that an assembly can be loaded with a full name.")]
        public void LoadAssemblyWithExistingFullName()
        {
            var helper = InitializeFusionHelper();
            var name = Assembly.GetExecutingAssembly().GetName().FullName;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame(Assembly.GetExecutingAssembly(), result, "Expected assembly with name {0} but got {1}", name, result);
        }

        [Test]
        [Description("Tests that an assembly cannot be loaded with a full name that misses the version number.")]
        public void LoadAssemblyWithNonExistingFullNameBasedOnVersion()
        {
            var helper = InitializeFusionHelper();
            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, new Version(0, 0, 0, 0), assemblyName.CultureInfo, new System.Text.ASCIIEncoding().GetString(assemblyName.GetPublicKeyToken()));
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        [Description("Tests that an assembly cannot be loaded with a full name that misses the culture.")]
        public void LoadAssemblyWithNonExistingFullNameBasedOnCulture()
        {
            var helper = InitializeFusionHelper();
            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, assemblyName.Version, new CultureInfo("en-US"), new System.Text.ASCIIEncoding().GetString(assemblyName.GetPublicKeyToken()));
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        [Description("Tests that an assembly cannot be loaded with a full name that misses the public key.")]
        public void LoadAssemblyWithNonExistingFullNameBasedOnPublicToken()
        {
            var helper = InitializeFusionHelper();
            var assemblyName = typeof(FusionHelper).Assembly.GetName();
            var name = CreateFullAssemblyName(assemblyName.Name, assemblyName.Version, assemblyName.CultureInfo, "null");
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }
    }
}
