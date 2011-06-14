//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Common.Scripting;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.UI.Scripting
{
    [TestFixture]
    [Description("Tests the ScriptErrorInformation class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptErrorInformationTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ScriptErrorInformation>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<ScriptErrorInformation> 
                        {
                            new ScriptErrorInformation 
                                { 
                                    Line = 1,
                                    Column = 1,
                                    Message = "a",
                                    Severity = SyntaxVerificationSeverity.Error
                                },
                            new ScriptErrorInformation 
                                { 
                                    Line = 2,
                                    Column = 1,
                                    Message = "a",
                                    Severity = SyntaxVerificationSeverity.Error
                                },
                            new ScriptErrorInformation 
                                { 
                                    Line = 1,
                                    Column = 2,
                                    Message = "a",
                                    Severity = SyntaxVerificationSeverity.Error
                                },
                            new ScriptErrorInformation 
                                { 
                                    Line = 1,
                                    Column = 1,
                                    Message = "b",
                                    Severity = SyntaxVerificationSeverity.Error
                                },
                            new ScriptErrorInformation 
                                { 
                                    Line = 1,
                                    Column = 1,
                                    Message = "a",
                                    Severity = SyntaxVerificationSeverity.Warning
                                },
                        },
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<ScriptErrorInformation>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new ScriptErrorInformation 
                                { 
                                    Line = 1,
                                    Column = 1,
                                    Message = "a",
                                    Severity = SyntaxVerificationSeverity.Error
                                },
                            new ScriptErrorInformation 
                                { 
                                    Line = 2,
                                    Column = 1,
                                    Message = "a",
                                    Severity = SyntaxVerificationSeverity.Error
                                },
                            new ScriptErrorInformation 
                                { 
                                    Line = 1,
                                    Column = 2,
                                    Message = "a",
                                    Severity = SyntaxVerificationSeverity.Error
                                },
                            new ScriptErrorInformation 
                                { 
                                    Line = 1,
                                    Column = 1,
                                    Message = "b",
                                    Severity = SyntaxVerificationSeverity.Error
                                },
                            new ScriptErrorInformation 
                                { 
                                    Line = 1,
                                    Column = 1,
                                    Message = "a",
                                    Severity = SyntaxVerificationSeverity.Warning
                                },
                    },
        };
    }
}
