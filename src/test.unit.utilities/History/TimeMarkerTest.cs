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
    public sealed class TimeMarkerTest
    {
        private static TimeMarker Create(ulong id)
        {
            return (TimeMarker)Mirror.ForType<TimeMarker>().Constructor.Invoke(id);
        }

        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<TimeMarker>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<TimeMarker> 
                    {
                        Create(0),
                        Create(1),
                        Create(2),
                        Create(3),
                        Create(4),
                        Create(5),
                        Create(6),
                        Create(7),
                        Create(8),
                        Create(9),
                    },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<TimeMarker>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    Create(0),
                    Create(1),
                    Create(2),
                    Create(3),
                    Create(4),
                    Create(5),
                    Create(6),
                    Create(7),
                    Create(8),
                    Create(9),
                },
        };

        [VerifyContract]
        public readonly IContract ComparableVerification = new ComparisonContract<TimeMarker>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection 
                { 
                    Create(0),
                    Create(1),
                    Create(2),
                    Create(3),
                    Create(4),
                },
        };
    }
}
