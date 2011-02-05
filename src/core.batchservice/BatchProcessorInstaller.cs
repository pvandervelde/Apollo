//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using Apollo.Core.BatchService.Properties;

namespace Apollo.Core.BatchService
{
    /// <summary>
    /// The installer for the <see cref="BatchProcessor"/>.
    /// </summary>
    /// <design>
    /// Note that this class is only meant to be used for local installs used
    /// by the developers. Proper installation of the service will be done with
    /// an install framework, e.g. WiX.
    /// </design>
    [RunInstaller(true)]
    internal sealed class BatchProcessorInstaller : Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchProcessorInstaller"/> class.
        /// </summary>
        public BatchProcessorInstaller()
        {
            var serviceProcessInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            // Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            // Service Information
            serviceInstaller.DisplayName = Resources.Service_Description;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // This must be identical to the WindowsService.ServiceBase name
            // set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = BatchProcessor.Name;

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
