//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.UI.Wpf
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ParameterTest : EqualityContractVerifierTest
    {
        private sealed class MockParameter1 : Parameter
        {
            public MockParameter1(IContextAware context)
                : base(context)
            {
            }
        }

        private sealed class MockParameter2 : Parameter
        {
            public MockParameter2(IContextAware context)
                : base(context)
            {
            }
        }

        private sealed class MockParameter3 : Parameter
        {
            public MockParameter3(IContextAware context)
                : base(context)
            {
            }
        }

        private sealed class ParameterEqualityContractVerifier : EqualityContractVerifier<Parameter>
        {
            private readonly Parameter m_First = new MockParameter1(new Mock<IContextAware>().Object);

            private readonly Parameter m_Second = new MockParameter2(new Mock<IContextAware>().Object);

            protected override Parameter Copy(Parameter original)
            {
                if (original.GetType() == typeof(MockParameter1))
                {
                    return new MockParameter1(new Mock<IContextAware>().Object);
                }

                return new MockParameter2(new Mock<IContextAware>().Object);
            }

            protected override Parameter FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override Parameter SecondInstance
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

        private sealed class ParameterHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<Parameter> m_DistinctInstances
                = new List<Parameter> 
                     {
                        new MockParameter1(new Mock<IContextAware>().Object),
                        new MockParameter2(new Mock<IContextAware>().Object),
                        new MockParameter3(new Mock<IContextAware>().Object),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ParameterHashcodeContractVerfier m_HashcodeVerifier = new ParameterHashcodeContractVerfier();

        private readonly ParameterEqualityContractVerifier m_EqualityVerifier = new ParameterEqualityContractVerifier();

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
