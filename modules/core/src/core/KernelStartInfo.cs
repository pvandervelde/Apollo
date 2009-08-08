using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Apollo.Core
{
    /// <summary>
    /// Stores information used by the starting process of the kernel.
    /// </summary>
    public sealed class KernelStartInfo
    {
        // Which assembly paths do we need?
        // - Core set
        // - Plug-ins (can be extended at run-time)
        // - UI paths
        // We load these from the config file. The base sets
        // are hard-coded (i.e. we can never change the directory structure)

        // Kernel, message service --> only need Apollo.Core (maybe Apollo.Utils, Autofac, ...)
        // LogSink --> Apollo.Core + logger assemblies
        // Persistence --> core + ??
        // License --> core + ??
        // Project, Plugin --> Core + plugin directories
        // UI --> Core + UI assemblies

        //private readonly IEnumerable<FileInfo> m_CoreSet;

        //private readonly IEnumerable<FileInfo> m_LicenseSet;

        //private readonly IEnumerable<FileInfo> m_UserInterfaceSet;

        //private readonly IEnumerable<DirectoryInfo> m_PluginDirectories;


        // Note that at some point in time we will need to be able to do shadow copying / loading
        // and we'll need to be able to load version specific assemblies.
    }
}
