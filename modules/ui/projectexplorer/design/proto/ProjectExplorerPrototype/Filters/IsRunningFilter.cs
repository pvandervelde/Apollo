using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectExplorerPrototype.Properties;

namespace ProjectExplorerPrototype.Filters
{
    public sealed class IsRunningFilter : Filter
    {
        public IsRunningFilter() : base(Resources.IsRunningFilter_DefaultName)
        { }

        public IsRunningFilter(bool isActive)
            : base(Resources.IsRunningFilter_DefaultName, isActive)
        { }
    }
}
