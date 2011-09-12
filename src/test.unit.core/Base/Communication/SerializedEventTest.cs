//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedEventTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedEvent>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedEvent> 
                        {
                            new SerializedEvent(new SerializedType("a"), "a"),
                            new SerializedEvent(new SerializedType("b"), "b"),
                            new SerializedEvent(new SerializedType("c"), "c"),
                            new SerializedEvent(new SerializedType("a"), "d"),
                            new SerializedEvent(new SerializedType("b"), "e"),
                            new SerializedEvent(new SerializedType("c"), "f"),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ISerializedEventRegistration>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new SerializedEvent(new SerializedType("a"), "a"),
                        new SerializedEvent(new SerializedType("b"), "b"),
                        new SerializedEvent(new SerializedType("c"), "c"),
                        new SerializedEvent(new SerializedType("a"), "d"),
                        new SerializedEvent(new SerializedType("b"), "e"),
                        new SerializedEvent(new SerializedType("c"), "f"),
                    },
        };
    }
}
