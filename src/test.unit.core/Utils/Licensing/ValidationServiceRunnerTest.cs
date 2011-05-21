//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.Licensing;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Utilities.Licensing
{
    [TestFixture]
    [Description("Tests the ValidationServiceRunner class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationServiceRunnerTest
    {
        [Test]
        [Description("Checks that the service is correctly started.")]
        public void Initialize()
        {
            var mockService = new Mock<IValidationService>();
            {
                mockService.Setup(s => s.StartValidation())
                    .Verifiable();
            }

            var runner = new ValidationServiceRunner(mockService.Object);

            runner.Initialize();
            mockService.Verify(s => s.StartValidation(), Times.Exactly(1));
        }
    }
}
