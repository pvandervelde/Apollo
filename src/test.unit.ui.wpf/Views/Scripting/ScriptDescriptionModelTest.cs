//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Host.Scripting;
using Moq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptDescriptionModelTest : EqualityContractVerifierTest
    {
        private sealed class ScriptDescriptionModelEqualityContractVerifier : EqualityContractVerifier<ScriptDescriptionModel>
        {
            private readonly ScriptDescriptionModel m_First 
                = new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.IronPython);

            private readonly ScriptDescriptionModel m_Second 
                = new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.IronRuby);

            protected override ScriptDescriptionModel Copy(ScriptDescriptionModel original)
            {
                return new ScriptDescriptionModel(new Mock<IContextAware>().Object, original.Language);
            }

            protected override ScriptDescriptionModel FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ScriptDescriptionModel SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class ScriptDescriptionModelHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ScriptDescriptionModel> m_DistinctInstances
                = new List<ScriptDescriptionModel> 
                     {
                        new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.IronPython),
                        new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.IronRuby),
                        new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.PowerShell),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ScriptDescriptionModelHashcodeContractVerfier m_HashcodeVerifier = new ScriptDescriptionModelHashcodeContractVerfier();

        private readonly ScriptDescriptionModelEqualityContractVerifier m_EqualityVerifier = new ScriptDescriptionModelEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }

        [Test]
        public void Create()
        {
            var context = new Mock<IContextAware>();

            var language = ScriptLanguage.IronPython;
            var model = new ScriptDescriptionModel(context.Object, language);

            Assert.AreEqual(language, model.Language);
            Assert.IsNotNull(model.Description);
            Assert.AreNotEqual(string.Empty, model.Description);
        }
    }
}
