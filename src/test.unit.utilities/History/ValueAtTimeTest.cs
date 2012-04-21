//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ValueAtTimeTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ValueAtTime<int>>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ValueAtTime<int>> 
                    {
                        new ValueAtTime<int>(new TimeMarker(10), 10),
                        new ValueAtTime<int>(new TimeMarker(20), 10),
                        new ValueAtTime<int>(new TimeMarker(30), 10),
                        new ValueAtTime<int>(new TimeMarker(40), 10),
                        new ValueAtTime<int>(new TimeMarker(50), 10),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ValueAtTime<int>>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new ValueAtTime<int>(new TimeMarker(10), 10),
                    new ValueAtTime<int>(new TimeMarker(20), 10),
                    new ValueAtTime<int>(new TimeMarker(30), 10),
                    new ValueAtTime<int>(new TimeMarker(40), 10),
                    new ValueAtTime<int>(new TimeMarker(50), 10),
                },
        };

        [VerifyContract]
        public readonly IContract ComparableVerification = new ComparisonContract<ValueAtTime<int>>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection 
                { 
                    new ValueAtTime<int>(new TimeMarker(10), 10),
                    new ValueAtTime<int>(new TimeMarker(20), 10),
                    new ValueAtTime<int>(new TimeMarker(30), 10),
                    new ValueAtTime<int>(new TimeMarker(40), 10),
                    new ValueAtTime<int>(new TimeMarker(50), 10),
                },
        };
    }
}
