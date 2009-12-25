// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.UserInterfaces.Application
{
    /// <summary>
    /// Defines the interface for objects that provide an interface abstraction to the
    /// application.
    /// </summary>
    public interface IAbstractApplications
    {
        /// <summary>
        /// Gets a value indicating whether the application can shutdown.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the application can shutdown; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanShutdown
        {
            get;
        }

        /// <summary>
        /// Shuts the application down, forcibly if asked.
        /// </summary>
        /// <param name="shouldBeForced">If set to <see langword="true"/> then the shutdown will be forced.</param>
        /// <param name="onShutdownRefuse">The <see cref="Action"/> that will be performed if the shutdown is refused.</param>
        void Shutdown(bool shouldBeForced, Action onShutdownRefuse);

        /// <summary>
        /// Gets the object that provides information about the application status.
        /// </summary>
        /// <value>The object that provides information about the application status.</value>
        IHoldSystemInformation ApplicationStatus
        {
            get;
        }
    }
}