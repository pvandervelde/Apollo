//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.History;
using NUnit.Framework;

namespace Apollo.Core.Dataset.Nuclei
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class HistoryMarkerStorageTest
    {
        [Test]
        public void Add()
        {
            var baseline = new List<TimeMarker> 
                {
                    new TimeMarker(10),
                    new TimeMarker(20),
                    new TimeMarker(30),
                };

            var collection = new HistoryMarkerStorage();
            foreach (var item in baseline)
            {
                collection.Add(item);
            }

            Assert.That(collection, Is.EquivalentTo(baseline));
        }
    }
}
