//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Scripting;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Views.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptModelTest
    {
        [Test]
        public void ScriptLanguage()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var model = new ScriptModel(context.Object);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            var description = new ScriptDescriptionModel(context.Object, Core.Host.Scripting.ScriptLanguage.IronRuby);
            model.ScriptLanguage = description;

            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "ScriptLanguage",
                    },
                properties);

            var newDescription = new ScriptDescriptionModel(context.Object, Core.Host.Scripting.ScriptLanguage.IronRuby);
            model.ScriptLanguage = newDescription;

            Assert.AreEqual(1, propertyChangedWasRaised);
        }

        [Test]
        public void ScriptVerifier()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var model = new ScriptModel(context.Object);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            var verifier = new Mock<ISyntaxVerifier>();
            {
                verifier.Setup(v => v.Equals(It.IsAny<object>()))
                    .Returns<object>(o => o != null);
            }

            model.SyntaxVerifier = verifier.Object;

            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "SyntaxVerifier",
                    },
                properties);

            model.SyntaxVerifier = verifier.Object;

            Assert.AreEqual(1, propertyChangedWasRaised);
        }

        [Test]
        public void ScriptFile()
        {
            var context = new Mock<IContextAware>();
            {
                context.Setup(c => c.IsSynchronized)
                    .Returns(true);
            }

            var model = new ScriptModel(context.Object);

            var propertyChangedWasRaised = 0;
            var properties = new List<string>();
            model.PropertyChanged += (s, e) =>
            {
                propertyChangedWasRaised++;
                properties.Add(e.PropertyName);
            };

            var path = "a";
            model.ScriptFile = path;

            Assert.AreEqual(1, propertyChangedWasRaised);
            Assert.AreElementsEqual(
                new List<string>
                    {
                        "ScriptFile",
                    },
                properties);

            var newPath = "a";
            model.ScriptFile = newPath;

            Assert.AreEqual(1, propertyChangedWasRaised);
        }
    }
}
