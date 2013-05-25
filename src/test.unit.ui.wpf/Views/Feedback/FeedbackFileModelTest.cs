//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;

namespace Apollo.UI.Wpf.Views.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FeedbackFileModelTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<FeedbackFileModel>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                new List<FeedbackFileModel> 
                        {
                            new FeedbackFileModel(new Mock<IContextAware>().Object, "a", DateTimeOffset.Now),
                            new FeedbackFileModel(new Mock<IContextAware>().Object, "b", DateTimeOffset.Now),
                            new FeedbackFileModel(new Mock<IContextAware>().Object, "c", DateTimeOffset.Now),
                            new FeedbackFileModel(new Mock<IContextAware>().Object, "d", DateTimeOffset.Now),
                            new FeedbackFileModel(new Mock<IContextAware>().Object, "e", DateTimeOffset.Now),
                        },
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<FeedbackFileModel>
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = new EquivalenceClassCollection
                    { 
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "a", DateTimeOffset.Now),
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "b", DateTimeOffset.Now),
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "c", DateTimeOffset.Now),
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "d", DateTimeOffset.Now),
                        new FeedbackFileModel(new Mock<IContextAware>().Object, "e", DateTimeOffset.Now),
                    },
        };

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
