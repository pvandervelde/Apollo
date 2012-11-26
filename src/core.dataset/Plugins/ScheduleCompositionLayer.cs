//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Scheduling;

namespace Apollo.Core.Dataset.Plugins
{
    internal sealed class ScheduleCompositionLayer
    {
        public void Add(ScheduleDefinition scheduleDefinition, GroupCompositionId owningGroup)
        {
            throw new NotImplementedException();
        }

        public void Remove(GroupCompositionId owningGroup)
        {
            throw new NotImplementedException();
        }

        public void Connect(GroupCompositionId importingGroup, EditableInsertVertex insertPoint, GroupCompositionId exportingGroup)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(GroupCompositionId group)
        {
            throw new NotImplementedException();
        }
    }
}
