//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.UI.Wpf.Views.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FeedbackFileModelTest : EqualityContractVerifierTest
    {
        private sealed class FeedbackFileModelEqualityContractVerifier : EqualityContractVerifier<FeedbackFileModel>
        {
            private readonly FeedbackFileModel m_First = new FeedbackFileModel(new Mock<IContextAware>().Object, "a", DateTimeOffset.Now);

            private readonly FeedbackFileModel m_Second = new FeedbackFileModel(new Mock<IContextAware>().Object, "b", DateTimeOffset.Now);

            protected override FeedbackFileModel Copy(FeedbackFileModel original)
            {
                return new FeedbackFileModel(new Mock<IContextAware>().Object, original.Path, original.Date);
            }

            protected override FeedbackFileModel FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override FeedbackFileModel SecondInstance
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

        private sealed class FeedbackFileModelHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<FeedbackFileModel> m_DistinctInstances
                = new List<FeedbackFileModel> 
                     {
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "a", DateTimeOffset.Now),
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "b", DateTimeOffset.Now),
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "c", DateTimeOffset.Now),
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "d", DateTimeOffset.Now),
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "e", DateTimeOffset.Now),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly FeedbackFileModelHashcodeContractVerfier m_HashcodeVerifier = new FeedbackFileModelHashcodeContractVerfier();

        private readonly FeedbackFileModelEqualityContractVerifier m_EqualityVerifier = new FeedbackFileModelEqualityContractVerifier();

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
            var file = "a";
            var time = DateTimeOffset.Now;

            var model = new FeedbackFileModel(context.Object, file, time);
            Assert.AreSame(file, model.Path);
            Assert.AreEqual(time, model.Date);
        }
    }
}
