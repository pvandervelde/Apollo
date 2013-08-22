//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Models
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MostRecentlyUsedCollectionTest
    {
        [Test]
        public void Add()
        {
            var mruCollection = new MostRecentlyUsedCollection();

            var paths = new List<string>();
            for (int i = 0; i <= mruCollection.MaximumSize; i++)
            {
                paths.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        @"c:\temp\myfile{0}.txt",
                        i));
            }

            for (int i = 0; i < paths.Count; i++)
            {
                mruCollection.Add(paths[i]);
                Assert.That(
                    mruCollection.Select(m => m.FilePath),
                    Is.EquivalentTo(
                        paths
                            .Skip(i >= mruCollection.MaximumSize ? i - mruCollection.MaximumSize + 1 : 0)
                            .Take(i >= mruCollection.MaximumSize ? mruCollection.MaximumSize : i + 1)
                            .Reverse()));
            }
        }

        [Test]
        public void SerializeAndDeserialize()
        {
            var mruCollection = new MostRecentlyUsedCollection();
            for (int i = 0; i <= mruCollection.MaximumSize; i++)
            {
                mruCollection.Add(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        @"c:\temp\myfile{0}.txt",
                        i));
            }

            var converter = new MostRecentlyUsedCollectionConverter();
            var serializedCollection = converter.ConvertTo(null, CultureInfo.InvariantCulture, mruCollection, typeof(string));
            var deserializedCollection = converter.ConvertFrom(
                null,
                CultureInfo.InvariantCulture,
                serializedCollection) as MostRecentlyUsedCollection;

            Assert.That(deserializedCollection.Select(m => m.FilePath), Is.EquivalentTo(mruCollection.Select(m => m.FilePath)));
            Assert.That(deserializedCollection.Select(m => m.LastTimeOpened), Is.EquivalentTo(mruCollection.Select(m => m.LastTimeOpened)));
        }
    }
}
