//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Apollo.Core.Base.Communication;
using Apollo.Utilities;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Loads datasets into an external application and provides enough information for the
    /// requester to connect to that application via the communication system.
    /// </summary>
    internal sealed class DatasetApplicationLoader : IApplicationLoader
    {
        /// <summary>
        /// Defines the file name for the dataset application.
        /// </summary>
        private const string DatasetApplicationFileName = @"Apollo.Core.Dataset.exe";

        private static Process StartDatasetApplication(EndpointId localEndpoint, Uri localConnection, ConversationToken token)
        {
            var fullFilePath = Path.Combine(Assembly.GetExecutingAssembly().LocalDirectoryPath(), DatasetApplicationFileName);
            
            // The arguments are:
            // - The endpoint ID of the current application
            // - The URL of the named pipe for the current application
            // - The string version of the token that is used to start a 'conversation'
            var arguments = string.Format(
                CultureInfo.InvariantCulture, 
                "--parent={0} --channel={1} --token={2}", 
                localEndpoint, 
                localConnection, 
                token);
            
            var startInfo = new ProcessStartInfo 
                {
                    FileName = fullFilePath,
                    
                    // For now no arguments .. there should be though ...
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

            return exec;
        }

        /// <summary>
        /// Loads the dataset into an external application and returns the requested communication
        /// information.
        /// </summary>
        /// <returns>The information describing the channel on which the new dataset application can be contacted.</returns>
        public ChannelConnectionInformation LoadDataset()
        {
            // Load a dataset application
            // Need to:
            // - pass a connection to the loader app so that we can provide it 
            //   with the info it needs?
            // - Start info includes:
            //   - The dataset it wants to load
            //   - The connection parameters to the app that requested the load
            //   - ??

            // Start the application
            var localEndpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var token = new ConversationToken(localEndpoint);
            var datasetApp = StartDatasetApplication(
                localEndpoint, 
                null, 
                token);

            // Wait for it to connect
            // Once it connects we need to provided it with:
            // - The connection information for the application that asked for the dataset to be 
            //   loaded (note that this may be us in a local loading situation)
            // - The token that can be used to talk to the remote app regarding the dataset that must
            //   be loaded
            //
            // The app needs to provide us with
            // - The connection info so that we can send that back
            return new ChannelConnectionInformation(datasetApp.CreateEndpointIdForProcess(), null, null);
        }
    }
}
