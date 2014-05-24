//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Apollo.Utilities;
using Nuclei;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Apollo.Core.Base.Activation
{
    /// <summary>
    /// Activates datasets into an external application and provides enough information for the
    /// requester to connect to that application via the communication system.
    /// </summary>
    internal sealed class DatasetActivator : IDatasetActivator
    {
        /// <summary>
        /// Defines the file name for the dataset application.
        /// </summary>
        private const string DatasetApplicationFileName = @"Apollo.Core.Dataset.exe";

        /// <summary>
        /// Defines the file name for the configuration file of the dataset application.
        /// </summary>
        private const string DatasetApplicationConfigFileName = DatasetApplicationFileName + @".config";

        /// <summary>
        /// The object that links the dataset processes to the current process so that they
        /// all die at the same time.
        /// </summary>
        private static readonly DatasetTrackingJob s_ProcessTrackingJob
            = new DatasetTrackingJob();

        private static string DeployLocation()
        {
            var companyPath = Path.Combine(Path.GetTempPath(), ApplicationConstants.CompanyName);
            var productPath = Path.Combine(companyPath, ApplicationConstants.ApplicationName);
            var versionPath = Path.Combine(productPath, ApplicationConstants.ApplicationCompatibilityVersion.ToString(2));
            if (!Directory.Exists(versionPath))
            {
                Directory.CreateDirectory(versionPath);
            }

            var deployPath = versionPath;
            while (Directory.Exists(deployPath))
            {
                var subPath = Path.GetRandomFileName();
                deployPath = Path.Combine(versionPath, subPath);
            }

            Directory.CreateDirectory(deployPath);
            return deployPath;
        }

        /// <summary>
        /// The collection that contains the file paths for all the local assemblies on which 
        /// the dataset application depends.
        /// </summary>
        private readonly List<string> m_DatasetApplicationDependencies
            = new List<string>();

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetActivator"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public DatasetActivator(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;

            LoadDatasetDependencies();
        }

        /// <summary>
        /// Extracts the full paths of the assemblies referenced by the dataset application and stores them locally.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In order to always have the correct set of references we load them from a pre-build XML file. Given that
        /// we cannot change any of the dependencies or their locations while the application is running we can
        /// pre-load the list.
        /// </para>
        /// <para>
        /// We could have done this through reflection but that would mean that we would have to load all the assemblies
        /// into the current AppDomain (which requires multiple file loads). This seems an excessive amount of file
        /// loading and on top of that we can't unload those assemblies anymore, so getting them from a pre-build 
        /// file seems wiser.
        /// </para>
        /// </remarks>
        private void LoadDatasetDependencies()
        {
            var localPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();
            var dependencyFile = Path.Combine(localPath, Path.GetFileNameWithoutExtension(DatasetApplicationFileName) + ".references.xml");
            var doc = XDocument.Load(dependencyFile);
            foreach (var reference in doc.Element("references").Elements("reference"))
            {
                var text = reference.Value;
                var assemblyName = new AssemblyName(text);
                var assemblyFile = Path.Combine(localPath, assemblyName.Name + ".dll");
                if (File.Exists(assemblyFile))
                {
                    m_DatasetApplicationDependencies.Add(assemblyFile);
                }
            }
        }

        /// <summary>
        /// Loads the dataset into an external application and returns when the dataset application has started.
        /// </summary>
        /// <param name="endpointId">The endpoint ID for the owner.</param>
        /// <param name="channelType">The type of channel on which the dataset should be contacted.</param>
        /// <param name="address">The channel address for the owner.</param>
        /// <returns>The ID number of the newly created endpoint.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="address"/> is <see langword="null" />.
        /// </exception>
        public EndpointId ActivateDataset(EndpointId endpointId, ChannelType channelType, Uri address)
        {
            {
                Lokad.Enforce.Argument(() => endpointId);
                Lokad.Enforce.Argument(() => address);
            }

            var deploymentDir = DeployLocation();
            m_Diagnostics.Log(
                LevelToLog.Debug,
                BaseConstants.LogPrefix,
                string.Format(CultureInfo.InvariantCulture, "Deploying to: {0}", deploymentDir));

            DeployApplication(deploymentDir);

            var fullFilePath = Path.Combine(deploymentDir, DatasetApplicationFileName);
            var arguments = string.Format(
                CultureInfo.InvariantCulture,
                @"--host={0} --channeltype=""{1}"" --channeluri={2}",
                endpointId,
                channelType,
                address);

            var startInfo = new ProcessStartInfo
            {
                FileName = fullFilePath,
                Arguments = arguments,

                // Create no window for the process. It doesn't need one
                // and we don't want the user to have a window they can 
                // kill. If the process has to die then the user will have
                // to put in some effort.
                CreateNoWindow = true,

                // Do not use the shell to create the application
                // This means we can only start executables.
                UseShellExecute = false,

                // Do not display an error dialog if startup fails
                // We'll deal with that ourselves
                ErrorDialog = false,

                // Do not redirect any of the input or output streams
                // We don't really care about them because we won't be using
                // them anyway.
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,

                // Set the working directory to something sane. Mostly the
                // directory of the file that we're trying to read.
                WorkingDirectory = deploymentDir,
            };

            using (var exec = new Process())
            {
                exec.StartInfo = startInfo;
                exec.Start();

                // Link to the current process so that the datasets die if we die
                s_ProcessTrackingJob.LinkChildProcessToJob(exec);

                return exec.CreateEndpointIdForProcess();
            }
        }

        /// <summary>
        /// Copy the dataset application to a temporary directory so that it can have it's own set of plugins and those
        /// plugins won't collide with other plugins of the same name but a different version.
        /// </summary>
        /// <param name="deployDirectory">The directory to which the application should be deployed.</param>
        private void DeployApplication(string deployDirectory)
        {
            var localPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();

            // copy all the application executables and assemblies
            var exeFiles = new[] 
                            {
                                Path.Combine(localPath, DatasetApplicationFileName),
                                Path.Combine(localPath, DatasetApplicationConfigFileName),
                            };

            var filesToDeploy = m_DatasetApplicationDependencies.Append(exeFiles);
            foreach (var file in filesToDeploy)
            {
                var localFile = Path.Combine(localPath, Path.GetFileName(file));
                var deployedFile = Path.Combine(deployDirectory, Path.GetFileName(file));

                m_Diagnostics.Log(
                    LevelToLog.Debug,
                    BaseConstants.LogPrefix,
                    string.Format(CultureInfo.InvariantCulture, "Deploying {0} to: {1}", file, deployedFile));
                File.Copy(localFile, deployedFile);

#if DEBUG
                // Copy the PDB files if we're in DEBUG mode
                const string debugFileExtension = ".pdb";

                var localDebugFile = Path.Combine(
                    localPath,
                    Path.GetFileNameWithoutExtension(file) + debugFileExtension);
                if (File.Exists(localDebugFile))
                {
                    var deployedDebugFile = Path.Combine(deployDirectory, Path.GetFileNameWithoutExtension(localFile) + debugFileExtension);
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        BaseConstants.LogPrefix,
                        string.Format(CultureInfo.InvariantCulture, "Deploying {0} to: {1}", localDebugFile, deployedDebugFile));
                    File.Copy(localDebugFile, deployedDebugFile);
                }
#endif
            }
        }
    }
}
