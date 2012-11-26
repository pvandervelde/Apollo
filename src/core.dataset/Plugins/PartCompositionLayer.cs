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
using Apollo.Core.Extensions.Plugins;

namespace Apollo.Core.Dataset.Plugins
{
    internal sealed class PartCompositionLayer
    {
        public void Add(PartCompositionId id, GroupPartDefinition part, GroupCompositionId owningGroup)
        {
            throw new NotImplementedException();
        }

        public void Remove(PartCompositionId id)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<PartCompositionId> parts)
        {
            throw new NotImplementedException();
        }

        public void Remove(GroupCompositionId group)
        {
            throw new NotImplementedException();
        }

        public GroupPartDefinition Part(PartCompositionId id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PartCompositionId> PartsByGroup(GroupCompositionId id)
        {
            throw new NotImplementedException();
        }

        public void Connect(
            PartCompositionId importingPart, 
            ImportRegistrationId import, 
            PartCompositionId exportingPart, 
            ExportRegistrationId export)
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

        public IEnumerable<Tuple<PartCompositionId, ExportRegistrationId>> ConnectedExports()
        {
            throw new NotImplementedException();
        }
    }
}
