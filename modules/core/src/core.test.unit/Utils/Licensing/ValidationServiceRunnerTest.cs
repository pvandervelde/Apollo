//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils.Licensing;
using MbUnit.Framework;

namespace Apollo.Core.Utils.Licensing
{
    [TestFixture]
    [Description("Tests the ValidationServiceRunner class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationServiceRunnerTest
    {
        #region internal class - MockValidationService

        private sealed class MockValidationService : IValidationService
        {
            public void StartValidation()
            {
                Started = true;
            }

            public bool Started
            {
                get;
                set;
            }
        }
        
        #endregion

        [Test]
        [Description("Checks that a service runner cannot be created with a null service reference.")]
        public void CreateWithNullService()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidationServiceRunner(null));
        }

        [Test]
        [Description("Checks that the service is correctly started.")]
        public void Initialize()
        {
            var mockService = new MockValidationService();
            var runner = new ValidationServiceRunner(mockService);

            runner.Initialize();
            Assert.IsTrue(mockService.Started);
        }
    }
}
