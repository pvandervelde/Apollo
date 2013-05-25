//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Apollo.UI.Wpf.Models;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Wpf.Views.Settings
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MostRecentlyUsedModelTest
    {
        [Test]
        public void Create()
        {
            var context = new Mock<IContextAware>();
            var mru = new MostRecentlyUsed(@"c:\temp\a.txt", DateTimeOffset.Now);
            var model = new MostRecentlyUsedModel(context.Object, mru);

            Assert.AreEqual(Path.GetFileNameWithoutExtension(mru.FilePath), model.FileName);
            Assert.AreEqual(mru.FilePath, model.FilePath);
            Assert.AreEqual(mru.LastTimeOpened, model.LastTimeOpened);
        }
    }
}
