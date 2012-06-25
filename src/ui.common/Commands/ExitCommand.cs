//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Application;
using Microsoft.Practices.Prism.Commands;

namespace Apollo.UI.Common.Commands
{
    /// <summary>
    /// Handles the exiting of the application.
    /// </summary>
    public sealed class ExitCommand : DelegateCommand<object>
    {
        /// <summary>
        /// Determines whether this application can exit.
        /// </summary>
        /// <param name="applicationFacade">
        /// The object that contains the methods that allow interaction
        /// with the kernel.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if this application can exit; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool CanExit(IAbstractApplications applicationFacade)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (applicationFacade == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called when an exit is requested.
        /// </summary>
        /// <param name="applicationFacade">
        /// The object that contains the methods that allow interaction
        /// with the kernel.
        /// </param>
        private static void OnExit(IAbstractApplications applicationFacade)
        {
            // If there is no application facade, then we're in 
            // designer mode, or something else silly.
            if (applicationFacade == null)
            {
                return;
            }

            // the only catch is that the Shutdown method will return before
            // we know if the shutdown will be canceled?
            applicationFacade.Shutdown();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommand"/> class.
        /// </summary>
        /// <param name="applicationFacade">
        /// The object that contains the methods that allow interaction
        /// with the kernel.
        /// </param>
        public ExitCommand(IAbstractApplications applicationFacade)
            : base(obj => OnExit(applicationFacade), obj => CanExit(applicationFacade))
        {
        }
    }
}
