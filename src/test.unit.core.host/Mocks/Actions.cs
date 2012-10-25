//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Plugins;

namespace Apollo.Core.Host.Mocks
{
    [Export("ActionExport")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ActionOnMethod
    {
        [ScheduleAction("ActionMethod")]
        public void ActionMethod()
        { 
        }
    }
}
