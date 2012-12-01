using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Core.Extensions.Plugins;
using QuickGraph;

namespace Apollo.Core.Dataset.Plugins
{
    internal sealed class PartCompositionGraphEdge : Edge<PartCompositionId>
    {
        private PartCompositionId partCompositionId1;
        private Extensions.Plugins.ImportRegistrationId importRegistrationId;
        private PartCompositionId partCompositionId2;
        private Extensions.Plugins.ExportRegistrationId export;

        public PartCompositionGraphEdge(
            PartCompositionId importPart, 
            ImportRegistrationId importId, 
            PartCompositionId exportPart, 
            ExportRegistrationId exportId)
            : base(exportPart, importPart)
        {
            // TODO: Complete member initialization
            foobar();
        }
    }
}
