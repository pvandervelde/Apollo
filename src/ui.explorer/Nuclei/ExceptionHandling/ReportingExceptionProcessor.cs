//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using NSarrac.Framework;

namespace Apollo.UI.Explorer.Nuclei.ExceptionHandling
{
    /// <summary>
    /// Processes exceptions and creates exception reports which are stored
    /// in a directory on the disk.
    /// </summary>
    /// <remarks>
    /// The exception reports are stored on the disk so that it is not necessary to try
    /// to send the reports when the application is in a corrupted state. Once the application
    /// restarts again then the reports will be send by another component.
    /// </remarks>
    internal sealed class ReportingExceptionProcessor : IExceptionProcessor
    {
        private static ApplicationData ApplicationData()
        {
            var assembly = Assembly.GetEntryAssembly();

            var attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            var name = (attributes.Length > 0 && attributes[0] is AssemblyProductAttribute)
                ? (attributes[0] as AssemblyProductAttribute).Product
                : "Unknown product";

            var version = assembly.GetName().Version;
            var culture = assembly.GetName().CultureInfo;

            return new ApplicationData(name, version, culture);
        }

        private static string LocalMachineCpuDescription()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_Processor");

                var builder = new StringBuilder();
                builder.Append("Processors:");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var description = queryObj["Description"];
                    var name = queryObj["Name"];

                    builder.Append(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            " - {0} ({1})", 
                            name, 
                            description));
                }

                return builder.ToString();
            }
            catch (ManagementException)
            {
                return "Unknown";
            }
        }

        private static ulong LocalMachinePhysicalMemorySizeInKiloBytes()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM CIM_OperatingSystem");

                ulong totalPhysical = 0;
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    totalPhysical += (ulong)queryObj["TotalVisibleMemorySize"];
                }

                return totalPhysical;
            }
            catch (ManagementException)
            {
                return 0;
            }
        }

        private static MachineData MachineData()
        {
            var cpu = LocalMachineCpuDescription();
            var memory = (int)(LocalMachinePhysicalMemorySizeInKiloBytes() / 1024);
            return new MachineData(cpu, memory);
        }

        private static OperatingSystemData OperatingSystemData()
        {
            var os = Environment.OSVersion;
            return new OperatingSystemData(os.Platform.ToString(), os.Version, CultureInfo.InstalledUICulture);
        }

        private static List<AssemblyData> AssemblyData()
        {
            var result = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         select new AssemblyData(assembly.GetName(), assembly.GlobalAssemblyCache);

            return result.ToList();
        }

        /// <summary>
        /// The function that returns the report builders.
        /// </summary>
        private readonly Func<IBuildReports> m_Builders;

        /// <summary>
        /// Indicates if the object has been disposed or not.
        /// </summary>
        private volatile bool m_WasDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingExceptionProcessor"/> class.
        /// </summary>
        /// <param name="builders">
        ///     The function that is used to create new report builders.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builders"/> is <see langword="null" />.
        /// </exception>
        public ReportingExceptionProcessor(Func<IBuildReports> builders)
        {
            {
                Lokad.Enforce.Argument(() => builders);
            }

            m_Builders = builders;
        }

        /// <summary>
        /// Processes the given exception.
        /// </summary>
        /// <param name="exception">The exception to process.</param>
        public void Process(Exception exception)
        {
            if (m_WasDisposed)
            {
                return;
            }

            var builder = m_Builders();
            var report = builder.ForError(new ErrorData(exception))
                    .AtTime(DateTimeOffset.Now, TimeZoneInfo.Local)
                    .InApplication(ApplicationData())
                    .OnMachine(MachineData())
                    .OnOperatingSystem(OperatingSystemData())
                    .WithAssemblies(AssemblyData())
                    .ToReport();

            var reportPath = ReportingUtilities.ProductSpecificApplicationDataDirectory();
            if (!Directory.Exists(reportPath))
            {
                Directory.CreateDirectory(reportPath);
            }

            // Write the GUID in registry format: {00000000-0000-0000-0000-000000000000}
            var fileName = ReportingUtilities.GenerateDumpFileName();
            using (var file = new FileStream(Path.Combine(reportPath, fileName), FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                report.CopyTo(file);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_WasDisposed)
            {
                return;
            }

            m_WasDisposed = true;
        }
    }
}
