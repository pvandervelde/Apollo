using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectExplorerPrototype.Properties;

namespace ProjectExplorerPrototype.Filters
{
    public class IsSystemGeneratedFilter : Filter
    {
        public IsSystemGeneratedFilter() : base(Resources.IsSystemGeneratedFilter_DefaultName)
        { }

        public IsSystemGeneratedFilter(bool isActive)
            : base(Resources.IsSystemGeneratedFilter_DefaultName, isActive)
        { }
    }
}
