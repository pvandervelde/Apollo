using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Wpf.Feedback;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.UI.Wpf.Views.Feedback
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class FailedToSendFeedbackReportExceptionTest
    {
        [VerifyContract]
        public readonly IContract ExceptionTests = new ExceptionContract<FailedToSendFeedbackReportException>
        {
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true,
        };
    }
}
