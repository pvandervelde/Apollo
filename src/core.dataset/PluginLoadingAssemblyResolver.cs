﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Nuclei.Communication;

namespace Apollo.Core.Dataset
{
    /// <summary>
    /// An assembly resolver that handles the transfer of plug-in assemblies from the 
    /// host application.
    /// </summary>
    internal sealed class PluginLoadingAssemblyResolver
    {
        /// <summary>
        /// The object that provides the commands registered with the application.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_HostCommands;

        /// <summary>
        /// The object that handles the communication with the remote host.
        /// </summary>
        private readonly DownloadDataFromRemoteEndpoints m_Layer;

        /// <summary>
        /// The object that virtualizes the file system.
        /// </summary>
        /// <remarks>
        /// Note that this object is here so that during testing we don't have to directly deal 
        /// with the file system.
        /// </remarks>
        private readonly IFileSystem m_FileSystem;

        /// <summary>
        /// The ID of the host endpoint.
        /// </summary>
        private readonly EndpointId m_HostId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginLoadingAssemblyResolver"/> class.
        /// </summary>
        /// <param name="hostCommands">The object that provides the commands registered with the application.</param>
        /// <param name="layer">The object that handles the communication with the remote host.</param>
        /// <param name="fileSystem">The object that virtualizes the file system.</param>
        /// <param name="hostId">The ID of the host endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hostCommands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="hostId"/> is <see langword="null" />.
        /// </exception>
        public PluginLoadingAssemblyResolver(
            ISendCommandsToRemoteEndpoints hostCommands,
            DownloadDataFromRemoteEndpoints layer,
            IFileSystem fileSystem,
            EndpointId hostId)
        {
            {
                Lokad.Enforce.Argument(() => hostCommands);
                Lokad.Enforce.Argument(() => layer);
                Lokad.Enforce.Argument(() => fileSystem);
                Lokad.Enforce.Argument(() => hostId);
            }

            m_HostCommands = hostCommands;
            m_Layer = layer;
            m_FileSystem = fileSystem;
            m_HostId = hostId;
        }

        /// <summary>
        /// An event handler which is invoked when the search for an assembly fails.
        /// </summary>
        /// <param name="sender">The object which raised the event.</param>
        /// <param name="args">
        ///     The <see cref="System.ResolveEventArgs"/> instance containing the event data.
        /// </param>
        /// <returns>
        ///     An assembly reference if the required assembly can be found; otherwise <see langword="null"/>.
        /// </returns>
        public Assembly LocatePluginAssembly(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            var commands = m_HostCommands.CommandsFor<IHostApplicationCommands>(m_HostId);
            var prepareTask = commands.PreparePluginContainerForTransfer(name);
            try
            {
                prepareTask.Wait();
            }
            catch (AggregateException)
            {
                return null;
            }

            var file = m_FileSystem.Path.GetTempFileName();
            try
            {
                var finalAssemblyPath = string.Format(CultureInfo.InvariantCulture, "{0}.dll", name.Name);

                var streamTask = m_Layer(m_HostId, prepareTask.Result, file);
                var copyTask = streamTask.ContinueWith(
                    t => m_FileSystem.File.Move(t.Result.FullName, finalAssemblyPath),
                    TaskContinuationOptions.ExecuteSynchronously);

                try
                {
                    copyTask.Wait();
                }
                catch (AggregateException)
                {
                    return null;
                }

                // The assembly should be here now so we should be able to load it
                return Assembly.Load(name);
            }
            finally
            {
                if (m_FileSystem.File.Exists(file))
                {
                    m_FileSystem.File.Delete(file);
                }
            }
        }
    }
}
