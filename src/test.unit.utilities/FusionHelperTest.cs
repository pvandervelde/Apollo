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
using MbUnit.Framework;
using Moq;

namespace Apollo.Utilities
{
    [TestFixture]
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

            // lokad
            m_Assemblies.Add(GetAssemblyPath(typeof(Lokad.Enforce).Assembly), typeof(Lokad.Enforce).Assembly);

            // NLog - This one is to verify a bug fix
            //   The bug behavior is that we couldn't locate this assembly because
            //   the public key token is: 5120e14c03d0593c but we were looking for
            //   5120e14c3d0593c (note the missing 0 in front of the c). This was 
            //   because we did the bit-to-hex for the public key token incorrectly.
            m_Assemblies.Add(GetAssemblyPath(typeof(NLog.Logger).Assembly), typeof(NLog.Logger).Assembly);

            // us
            m_Assemblies.Add(GetAssemblyPath(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly());
        }

        private FusionHelper InitializeFusionHelper()
        {
            var mockFileConstants = new Mock<IFileConstants>();
            mockFileConstants.Setup(constants => constants.AssemblyExtension)
                .Returns("dll");

            // Can effectively just return the current assembly / gallio assemblies / system
            var helper = new FusionHelper(() => m_Assemblies.Keys.ToArray<string>(), mockFileConstants.Object);
            helper.AssemblyLoader = (assemblyPath) =>
            {
                return m_Assemblies[assemblyPath];
            };

            return helper;
        }

        [Test]
        public void LoadAssemblyWithExistingSimpleName()
        {
            var helper = InitializeFusionHelper();
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingSimpleName()
        {
            var helper = InitializeFusionHelper();
            var name = typeof(FusionHelper).Assembly.GetName().Name;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithExistingSimpleNameWithExtension()
        {
            var helper = InitializeFusionHelper();
            var name = Assembly.GetExecutingAssembly().GetName().Name + ".dll";
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame(Assembly.GetExecutingAssembly(), result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingSimpleNameWithExtension()
        {
            var helper = InitializeFusionHelper();
            var name = typeof(FusionHelper).Assembly.GetName().Name + ".dll";
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithExistingFullName()
        {
            var helper = InitializeFusionHelper();
            var name = Assembly.GetExecutingAssembly().GetName().FullName;
            var result = ExecuteLoadAssembly(helper, name);
            Assert.AreSame(Assembly.GetExecutingAssembly(), result, "Expected assembly with name {0} but got {1}", name, result);
        }

        [Test]
        public void LoadAssemblyBasedOnPublicToken()
        {
            var helper = InitializeFusionHelper();
            Assert.AreSame(typeof(Lokad.Enforce).Assembly, ExecuteLoadAssembly(helper, typeof(Lokad.Enforce).Assembly.FullName));
            Assert.AreSame(typeof(TestAttribute).Assembly, ExecuteLoadAssembly(helper, typeof(TestAttribute).Assembly.FullName));
            Assert.AreSame(typeof(NLog.Logger).Assembly, ExecuteLoadAssembly(helper, typeof(NLog.Logger).Assembly.FullName));
            Assert.AreSame(typeof(string).Assembly, ExecuteLoadAssembly(helper, typeof(string).Assembly.FullName));
        }

        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnVersion()
        {
            var helper = InitializeFusionHelper();
            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(
                assemblyName.Name, 
                new Version(0, 0, 0, 0), 
                assemblyName.CultureInfo, 
                new System.Text.ASCIIEncoding().GetString(assemblyName.GetPublicKeyToken()));
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
        public void LoadAssemblyWithNonExistingFullNameBasedOnCulture()
        {
            var helper = InitializeFusionHelper();
            var assemblyName = typeof(TestAttribute).Assembly.GetName();
            var name = CreateFullAssemblyName(
                assemblyName.Name, 
                assemblyName.Version, 
                new CultureInfo("en-US"), 
                new System.Text.ASCIIEncoding().GetString(assemblyName.GetPublicKeyToken()));
            var result = ExecuteLoadAssembly(helper, name);
            Assert.IsNull(result);
        }

        [Test]
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
