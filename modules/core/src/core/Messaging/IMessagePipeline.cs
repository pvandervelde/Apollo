//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core.Messaging
{
    /// <summary>
    /// Defines the interface for objects that act as the center of the 
    /// message sending system.
    /// </summary>
    public interface IMessagePipeline
    {
        /// <summary>
        /// Determines whether a service with the specified name is registered.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>
        ///     <see langword="true"/> if a service with the specified name is registered; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsRegistered(DnsName name);

        /// <summary>
        /// Determines whether the specified service is registered.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified service is registered; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsRegistered(IProcessMessages service);

        /// <summary>
        /// Determines whether the specified service is registered.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified service is registered; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsRegistered(ISendMessages service);

        /// <summary>
        /// Registers as listener.
        /// </summary>
        /// <param name="service">The service.</param>
        void RegisterAsListener(IProcessMessages service);

        /// <summary>
        /// Registers as sender.
        /// </summary>
        /// <param name="service">The service.</param>
        void RegisterAsSender(ISendMessages service);

        /// <summary>
        /// Registers the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        void Register(object service);

        /// <summary>
        /// Unregisters the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        void Unregister(object service);

        /// <summary>
        /// Unregisters as listener.
        /// </summary>
        /// <param name="service">The service.</param>
        void UnregisterAsListener(IProcessMessages service);

        /// <summary>
        /// Unregisters as sender.
        /// </summary>
        /// <param name="service">The service.</param>
        void UnregisterAsSender(ISendMessages service);

        /// <summary>
        /// Sends the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information.</param>
        /// <returns>The ID number of the newly send message.</returns>
        MessageId Send(DnsName sender, DnsName recipient, MessageBody information);

        /// <summary>
        /// Sends the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="information">The information.</param>
        /// <param name="inReplyTo">The in reply to.</param>
        /// <returns>The ID number of the newly send message.</returns>
        MessageId Send(DnsName sender, DnsName recipient, MessageBody information, MessageId inReplyTo);
    }
}
