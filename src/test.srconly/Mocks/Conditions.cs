﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Extensions.Plugins;

namespace Test.Mocks
{
    [Export("ConditionOnMethodExport")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ConditionOnMethod
    {
        [ScheduleCondition("OnMethod")]
        public bool ConditionMethod()
        {
            return true;
        }
    }

    [Export("ConditionOnPropertyExport")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ConditionOnProperty
    {
        [ScheduleCondition("OnProperty")]
        public bool ConditionProperty
        {
            get
            {
                return true;
            }
        }
    }
}
