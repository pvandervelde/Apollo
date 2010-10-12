//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Concordion.Integration;

namespace Apollo.UI.Common.Spec.System
{
    [ConcordionTest]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Specification tests do not need documentation.")]
    public sealed class StartupTest
    {
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "For Concordion the fields must be public.")]
        public long FirstOperand;

        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
            Justification = "For Concordion the fields must be public.")]
        public long SecondOperand;

        public long Multiplication(long first, long second)
        {
            return -1;
        }
    }
}
