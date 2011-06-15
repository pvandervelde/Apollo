//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Common;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.UI
{
    [TestFixture]
    [Description("Tests the Parameter class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ParameterTest
    {
        private sealed class MockParameter1 : Parameter 
        { 
        }

        private sealed class MockParameter2 : Parameter 
        { 
        }

        private sealed class MockParameter3 : Parameter 
        { 
        }

        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<Parameter>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<Parameter> 
                        {
                            new MockParameter1(),
                            new MockParameter2(),
                            new MockParameter3(),
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<Parameter>
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new MockParameter1(),
                        new MockParameter2(),
                        new MockParameter3(),
                    },
        };
    }
}
