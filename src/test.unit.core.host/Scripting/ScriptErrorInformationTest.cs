//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Core.Host.Scripting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptErrorInformationTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<ScriptErrorInformation>
        {
            private readonly ScriptErrorInformation m_First = new ScriptErrorInformation
                {
                    Line = 1,
                    Column = 1,
                    Message = "a",
                    Severity = SyntaxVerificationSeverity.Error
                };

            private readonly ScriptErrorInformation m_Second = new ScriptErrorInformation
                {
                    Line = 2,
                    Column = 2,
                    Message = "b",
                    Severity = SyntaxVerificationSeverity.Error
                };

            protected override ScriptErrorInformation Copy(ScriptErrorInformation original)
            {
                return new ScriptErrorInformation
                    {
                        Line = original.Line,
                        Column = original.Column,
                        Message = original.Message,
                        Severity = original.Severity,
                    };
            }

            protected override ScriptErrorInformation FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ScriptErrorInformation SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class EndpointIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ScriptErrorInformation> m_DistinctInstances
                = new List<ScriptErrorInformation> 
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
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly EndpointIdHashcodeContractVerfier m_HashcodeVerifier = new EndpointIdHashcodeContractVerfier();

        private readonly EndpointIdEqualityContractVerifier m_EqualityVerifier = new EndpointIdEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }
    }
}
