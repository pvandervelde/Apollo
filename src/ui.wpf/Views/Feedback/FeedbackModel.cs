//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Management;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Apollo.UI.Wpf.Feedback;
using NSarrac.Framework;

namespace Apollo.UI.Wpf.Views.Feedback
{
    /// <summary>
    /// Defines the model for a single feedback entry.
    /// </summary>
    public sealed class FeedbackModel : Model
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
                var searcher = new ManagementObjectSearcher(
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
                var searcher = new ManagementObjectSearcher(
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

        /// <summary>
        /// The command used to send the report.
        /// </summary>
        private readonly ICommand m_SendReportsCommand;

        /// <summary>
        /// The function that returns the report builders.
        /// </summary>
        private readonly Func<IBuildReports> m_Builders;

        /// <summary>
        /// The level for the feedback item.
        /// </summary>
        private FeedbackLevel m_Level;

        /// <summary>
        /// The description for the feedback item.
        /// </summary>
        private string m_Description;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="sendReportsCommand">The command that is used to send the feedback reports to the remote server.</param>
        /// <param name="builders">The function that is used to create new report builders.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendReportsCommand"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builders"/> is <see langword="null" />.
        /// </exception>
        public FeedbackModel(IContextAware context, ICommand sendReportsCommand, Func<IBuildReports> builders)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => sendReportsCommand);
                Lokad.Enforce.Argument(() => builders);
            }

            m_SendReportsCommand = sendReportsCommand;
            m_Builders = builders;
        }

        /// <summary>
        /// Gets a value indicating whether the feedback report can be send.
        /// </summary>
        public bool CanSendReport
        {
            get
            {
                return Level != FeedbackLevel.None;
            }
        }

        /// <summary>
        /// Sends the report to the remote server.
        /// </summary>
        public void SendReport()
        {
            var builder = m_Builders();
            var report = builder.WithFeedback(new FeedbackData(Level, Description))
                .AtTime(DateTimeOffset.Now, TimeZoneInfo.Local)
                .InApplication(ApplicationData())
                .OnMachine(MachineData())
                .OnOperatingSystem(OperatingSystemData())
                .ToReport();

            try
            {
                m_SendReportsCommand.Execute(report);
            }
            catch (FailedToSendFeedbackReportException)
            {
                // Just ignore it.
            }

            Clear();
        }

        /// <summary>
        /// Clears the data stored in the model.
        /// </summary>
        public void Clear()
        {
            Level = FeedbackLevel.None;
            Description = string.Empty;
        }

        /// <summary>
        /// Gets or sets the feedback level.
        /// </summary>
        public FeedbackLevel Level
        {
            get 
            {
                return m_Level;
            }
            
            set 
            {
                m_Level = value;
                Notify(() => Level);
                Notify(() => CanSendReport);
            }
        }

        /// <summary>
        /// Gets or sets the description for the feedback report.
        /// </summary>
        public string Description
        {
            get
            {
                return m_Description;
            }

            set
            {
                m_Description = value;
                Notify(() => Description);
            }
        }
    }
}
