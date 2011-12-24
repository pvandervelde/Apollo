//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Loads datasets into an external application and provides enough information for the
    /// requester to connect to that application via the communication system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class DatasetApplicationLoader : IApplicationLoader
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
        /// Loads the dataset into an external application and returns when the dataset application has started.
        /// </summary>
        /// <param name="ownerConnection">
        ///     The channel connection information for the owner.
        /// </param>
        /// <returns>The ID of the new endpoint.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="ownerConnection"/> is <see langword="null" />.
        /// </exception>
        public EndpointId LoadDataset(ChannelConnectionInformation ownerConnection)
        {
            {
                Enforce.Argument(() => ownerConnection);
            }

            var fullFilePath = Path.Combine(Assembly.GetExecutingAssembly().LocalDirectoryPath(), DatasetApplicationFileName);
            var arguments = string.Format(
                CultureInfo.InvariantCulture,
                @"--host={0} --channeltype=""{1}"" --channeluri={2}",
                ownerConnection.Id,
                ownerConnection.ChannelType.AssemblyQualifiedName,
                ownerConnection.Address);

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
                WorkingDirectory = Directory.GetCurrentDirectory(),
            };

            var exec = new Process();
            exec.StartInfo = startInfo;
            exec.Start();

            // Link to the current process so that the datasets die if we die
            s_ProcessTrackingJob.LinkChildProcessToJob(exec);

            return exec.CreateEndpointIdForProcess();
        }
    }
}
