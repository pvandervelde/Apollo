//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Moq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetDetailParameterTest : EqualityContractVerifierTest
    {
        private sealed class DatasetDetailParameterEqualityContractVerifier : EqualityContractVerifier<DatasetDetailParameter>
        {
            private readonly DatasetDetailParameter m_First = CreateInstance();

            private readonly DatasetDetailParameter m_Second = CreateInstance();

            protected override DatasetDetailParameter Copy(DatasetDetailParameter original)
            {
                return new DatasetDetailParameter(new Mock<IContextAware>().Object, original.Dataset);
            }

            protected override DatasetDetailParameter FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override DatasetDetailParameter SecondInstance
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

        private sealed class DatasetDetailParameterHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<DatasetDetailParameter> m_DistinctInstances
                = new List<DatasetDetailParameter> 
                     {
                        CreateInstance(),
                        CreateInstance(),
                        CreateInstance(),
                        CreateInstance(),
                        CreateInstance(),
                        CreateInstance(),
                        CreateInstance(),
                        CreateInstance(),
                        CreateInstance(),
                        CreateInstance(),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private static DatasetDetailParameter CreateInstance()
        {
            var context = new Mock<IContextAware>();
            var proxy = new Mock<IProxyDataset>();
            {
                proxy.Setup(p => p.Equals(It.IsAny<IProxyDataset>()))
                    .Returns<IProxyDataset>(other => ReferenceEquals(other, proxy.Object));
            }

            var dataset = new DatasetFacade(proxy.Object);
            return new DatasetDetailParameter(context.Object, dataset);
        }

        private readonly DatasetDetailParameterHashcodeContractVerfier m_HashcodeVerifier = new DatasetDetailParameterHashcodeContractVerfier();

        private readonly DatasetDetailParameterEqualityContractVerifier m_EqualityVerifier = new DatasetDetailParameterEqualityContractVerifier();

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

        [Test]
        public void Create()
        {
            var context = new Mock<IContextAware>();
            var proxy = new Mock<IProxyDataset>();

            var dataset = new DatasetFacade(proxy.Object);
            var parameter = new DatasetDetailParameter(context.Object, dataset);

            Assert.AreSame(dataset, parameter.Dataset);
        }
    }
}
