﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
    public sealed class SerializedMethodInvocationTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<SerializedMethodInvocation>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<SerializedMethodInvocation> 
                        {
                            new SerializedMethodInvocation(
                                new SerializedType("a"), 
                                "a", 
                                new List<Tuple<ISerializedType, object>>()),
                            new SerializedMethodInvocation(
                                new SerializedType("b"), 
                                "b", 
                                new List<Tuple<ISerializedType, object>>()),
                            new SerializedMethodInvocation(
                                new SerializedType("c"), 
                                "c", 
                                new List<Tuple<ISerializedType, object>>()),
                            new SerializedMethodInvocation(
                                new SerializedType("a"), 
                                "a", 
                                new List<Tuple<ISerializedType, object>> 
                                    { 
                                        new Tuple<ISerializedType, object>(new SerializedType("a"), "a")
                                    }),
                            new SerializedMethodInvocation(
                                new SerializedType("b"), 
                                "b", 
                                new List<Tuple<ISerializedType, object>> 
                                    { 
                                        new Tuple<ISerializedType, object>(new SerializedType("b"), "b")
                                    }),
                            new SerializedMethodInvocation(
                                new SerializedType("c"), 
                                "c", 
                                new List<Tuple<ISerializedType, object>> 
                                    { 
                                        new Tuple<ISerializedType, object>(new SerializedType("c"), "c")
                                    }),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ISerializedMethodInvocation>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new SerializedMethodInvocation(
                                new SerializedType("a"), 
                                "a", 
                                new List<Tuple<ISerializedType, object>>()),
                        new SerializedMethodInvocation(
                            new SerializedType("b"), 
                            "b", 
                            new List<Tuple<ISerializedType, object>>()),
                        new SerializedMethodInvocation(
                            new SerializedType("c"), 
                            "c", 
                            new List<Tuple<ISerializedType, object>>()),
                        new SerializedMethodInvocation(
                            new SerializedType("a"), 
                            "a", 
                            new List<Tuple<ISerializedType, object>> 
                                { 
                                    new Tuple<ISerializedType, object>(new SerializedType("a"), "a")
                                }),
                        new SerializedMethodInvocation(
                            new SerializedType("b"), 
                            "b", 
                            new List<Tuple<ISerializedType, object>> 
                                { 
                                    new Tuple<ISerializedType, object>(new SerializedType("b"), "b")
                                }),
                        new SerializedMethodInvocation(
                            new SerializedType("c"), 
                            "c", 
                            new List<Tuple<ISerializedType, object>> 
                                { 
                                    new Tuple<ISerializedType, object>(new SerializedType("c"), "c")
                                }),
                    },
        };
    }
}
