//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Apollo.ProjectExplorer.Properties;
using Apollo.ProjectExplorer.Views.Welcome;
using Apollo.UI.Common;
using Apollo.UI.Common.Commands;
using Autofac;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.ProjectExplorer.Views
{
    /// <summary>
    /// Builds composite commands.
    /// </summary>
    internal static class CompositeCommandBuilder
    {
        private sealed class CommandSwitchWrapper : DelegateCommand<object>
        {
            private static bool CanExecute(Func<bool> shouldExecute, ICommand command)
            {
                if (shouldExecute())
                {
                    return command.CanExecute(null);
                }

                return true;
            }

            private static void OnExecute(Func<bool> shouldExecute, ICommand commandToExecute)
            {
                if (shouldExecute())
                {
                    commandToExecute.Execute(null);
                }
            }

            public CommandSwitchWrapper(Func<bool> shouldExecute, ICommand commandToExecute)
                : base(obj => OnExecute(shouldExecute, commandToExecute), obj => CanExecute(shouldExecute, commandToExecute))
            { 
            }
        }

        /// <summary>
        /// Closes the welcome view and invokes the given command.
        /// </summary>
        /// <typeparam name="T">The type of command that should be invoked.</typeparam>
        /// <param name="container">The IOC container that contains all the UI controls.</param>
        /// <returns>The desired command.</returns>
        public static CompositeCommand CloseWelcomeViewAndInvokeCommand<T>(IContainer container) where T : ICommand
        {
            var compositeCommand = new CompositeCommand();

            var command = container.Resolve<T>();
            compositeCommand.RegisterCommand(command);

            var context = container.Resolve<IContextAware>();
            var closeViewCommand = container.Resolve<CloseViewCommand>(
                new TypedParameter(typeof(IEventAggregator), container.Resolve<IEventAggregator>()),
                new TypedParameter(typeof(string), CommonRegionNames.Content),
                new TypedParameter(typeof(Parameter), new WelcomeParameter(context)));
            compositeCommand.RegisterCommand(
                new CommandSwitchWrapper(
                    () => Settings.Default.CloseWelcomePageAfterProjectLoad,
                    closeViewCommand));

            return compositeCommand;
        }
    }
}
