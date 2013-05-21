//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;

namespace Apollo.UI.Wpf.Views.Datasets
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetDetailParameterTest
    {
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

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<Parameter>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
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
                },
        };

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
