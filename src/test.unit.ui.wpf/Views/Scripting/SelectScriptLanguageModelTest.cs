//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Views.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SelectScriptLanguageModelTest
    {
        [Test]
        public void AvailableLanguages()
        {
            var context = new Mock<IContextAware>();

            SelectScriptLanguageModel.StoreKnownLanguages(context.Object);
            var model = new SelectScriptLanguageModel(context.Object);

            Assert.IsNotNull(model.AvailableLanguages);
            Assert.LessThan(0, model.AvailableLanguages.Count);
        }

        [Test]
        public void SelectedLanguage()
        {
            var context = new Mock<IContextAware>();

            SelectScriptLanguageModel.StoreKnownLanguages(context.Object);
            var model = new SelectScriptLanguageModel(context.Object);

            model.SelectedLanguage = model.AvailableLanguages.First();
            Assert.IsNotNull(model.SelectedLanguage);
            Assert.AreEqual(model.AvailableLanguages.First(), model.SelectedLanguage);
        }
    }
}
