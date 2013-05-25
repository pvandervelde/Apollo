//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Scripting;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;

namespace Apollo.UI.Wpf.Views.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptDescriptionModelTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ScriptDescriptionModel>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ScriptDescriptionModel> 
                        {
                            new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.IronPython),
                            new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.IronRuby),
                            new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.PowerShell),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ScriptDescriptionModel>
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.IronPython),
                        new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.IronRuby),
                        new ScriptDescriptionModel(new Mock<IContextAware>().Object, ScriptLanguage.PowerShell),
                    },
        };

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
