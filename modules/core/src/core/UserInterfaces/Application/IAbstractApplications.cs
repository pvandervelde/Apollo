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
        /// <returns>
        ///     <see langword="true"/> if the application can shutdown; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool CanShutdown();

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

        /// <summary>
        /// Registers the notification.
        /// </summary>
        /// <param name="name">The name of the notification.</param>
        /// <param name="callback">The callback method that is called when the notification is activated.</param>
        void RegisterNotification(NotificationName name, Action<INotificationArguments> callback);
    }
}