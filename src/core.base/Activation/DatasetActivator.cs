﻿//-----------------------------------------------------------------------
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
        /// The object that links the dataset processes to the current process so that they
        /// all die at the same time.
        /// </summary>
        private static readonly DatasetTrackingJob s_ProcessTrackingJob
            = new DatasetTrackingJob();

        /// <summary>
        /// The object that stores the application wide constants.
        /// </summary>
        private readonly ApplicationConstants m_ApplicationConstants;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetActivator"/> class.
        /// </summary>
        /// <param name="applicationConstants">The object that stores the application wide constants.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="applicationConstants"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public DatasetActivator(ApplicationConstants applicationConstants, SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => applicationConstants);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_ApplicationConstants = applicationConstants;
            m_Diagnostics = diagnostics;
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

            var exec = new Process();
            exec.StartInfo = startInfo;
            exec.Start();

            // Link to the current process so that the datasets die if we die
            s_ProcessTrackingJob.LinkChildProcessToJob(exec);

            return exec.CreateEndpointIdForProcess();
        }

        private string DeployLocation()
        {
            var companyPath = Path.Combine(Path.GetTempPath(), m_ApplicationConstants.CompanyName);
            var productPath = Path.Combine(companyPath, m_ApplicationConstants.ApplicationName);
            var versionPath = Path.Combine(productPath, m_ApplicationConstants.ApplicationCompatibilityVersion.ToString(2));
            if (!Directory.Exists(versionPath))
            {
                Directory.CreateDirectory(versionPath);
            }

            string deployPath = versionPath;
            while (Directory.Exists(deployPath))
            {
                var subPath = Path.GetRandomFileName();
                deployPath = Path.Combine(versionPath, subPath);
            }

            Directory.CreateDirectory(deployPath);
            return deployPath;
        }

        private void DeployApplication(string deployDirectory)
        {
            const string exeFileExtension = "exe";
            const string configFileExtension = "exe.config";
            const string assemblyFileExtension = "dll";

            var executables = new List<string>
                {
                    "Apollo.Core.Dataset",
                };

            var assemblies = new List<string>
                {
                    "Apollo.Core.Base",
                    "Apollo.Core.Extensions",
                    "Apollo.Utilities",
                    "Autofac",
                    "Castle.Core",
                    "Lokad.Shared",
                    "Mono.Options",
                    "NLog",
                    "NManto",
                    "NSarrac.Framework",
                    "QuickGraph",
                    "System.Reactive",
                };

            var localPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();

            // copy all the application executables and assemblies
            var assemblyFiles = from assemblyFile in assemblies
                                select string.Format(CultureInfo.InvariantCulture, "{0}.{1}", assemblyFile, assemblyFileExtension);

            var exeFiles = from exeFile in executables
                           from file in new[] 
                            {
                                string.Format(CultureInfo.InvariantCulture, "{0}.{1}", exeFile, exeFileExtension),
                                string.Format(CultureInfo.InvariantCulture, "{0}.{1}", exeFile, configFileExtension),
                            }
                           select file;

            var filesToDeploy = assemblyFiles.Append(exeFiles);
            foreach (var file in filesToDeploy)
            {
                var localFile = Path.Combine(localPath, file);
                var deployedFile = Path.Combine(deployDirectory, file);

                m_Diagnostics.Log(
                    LevelToLog.Debug,
                    BaseConstants.LogPrefix,
                    string.Format(CultureInfo.InvariantCulture, "Deploying {1} to: {0}", file, deployedFile));
                File.Copy(localFile, deployedFile);
            }

#if DEBUG
            // Copy the PDB files if we're in DEBUG mode
            const string debugFileExtension = "pdb";

            var debugFiles = from file in assemblies.Append(executables)
                             select string.Format(CultureInfo.InvariantCulture, "{0}.{1}", file, debugFileExtension);
            foreach (var file in debugFiles)
            {
                var localFile = Path.Combine(localPath, file);
                var deployedFile = Path.Combine(deployDirectory, file);
                if (File.Exists(localFile))
                {
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        BaseConstants.LogPrefix,
                        string.Format(CultureInfo.InvariantCulture, "Deploying {1} to: {0}", file, deployedFile));
                    File.Copy(localFile, deployedFile);
                }
            }
#endif
        }
    }
}