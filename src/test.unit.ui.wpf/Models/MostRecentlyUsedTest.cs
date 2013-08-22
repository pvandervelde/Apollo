//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Models
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MostRecentlyUsedTest
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            var path = @"c:\temp\myfile.txt";
            var date = DateTimeOffset.Now;
            var mru = new MostRecentlyUsed(path, date);

            var converter = new MostRecentlyUsedConverter();
            var serializedMru = converter.ConvertTo(null, CultureInfo.InvariantCulture, mru, typeof(string));
            var deserializedMru = converter.ConvertFrom(null, CultureInfo.InvariantCulture, serializedMru) as MostRecentlyUsed;

            Assert.IsNotNull(deserializedMru);
            Assert.AreEqual(mru.FilePath, deserializedMru.FilePath);
            Assert.AreEqual(mru.LastTimeOpened, deserializedMru.LastTimeOpened);
        }
    }
}
