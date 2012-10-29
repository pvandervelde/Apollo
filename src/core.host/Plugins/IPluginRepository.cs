//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.Core.Base.Plugins;

namespace Apollo.Core.Host.Plugins
{
    internal interface IPluginRepository
    {
        IEnumerable<PluginFileInfo> KnownPluginFiles();

        void RemovePlugins(IEnumerable<string> deletedFiles);

        void Store(IEnumerable<PluginInfo> plugins, IEnumerable<TypeDefinition> types);

        // When we store the different plug-ins then we check:
        // - All types are resolved
        // - All constructor imports can be resolved
        // 
        // For groups we check
        // - Transformations do not link to data storage directly
        // - Single transformation function is known. Takes one or more inputs and produces a single output
    }
}
