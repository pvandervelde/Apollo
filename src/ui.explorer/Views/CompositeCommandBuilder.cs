//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;
using Apollo.UI.Explorer.Properties;
using Apollo.UI.Explorer.Views.Welcome;
using Apollo.UI.Wpf;
using Apollo.UI.Wpf.Commands;
using Apollo.Utilities;
using Autofac;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

namespace Apollo.UI.Explorer.Views
{
    /// <summary>
    /// Builds composite commands.
    /// </summary>
    internal static class CompositeCommandBuilder
    {
        private sealed class CommandSwitchWrapper : DelegateCommand<object>
        {
            private static void OnExecute(Func<bool> shouldExecute, ICommand commandToExecute)
            {
                if (shouldExecute() && commandToExecute.CanExecute(null))
                {
                    commandToExecute.Execute(null);
                }
            }

            public CommandSwitchWrapper(Func<bool> shouldExecute, ICommand commandToExecute)
                : base(obj => OnExecute(shouldExecute, commandToExecute))
            { 
            }
        }

        /// <summary>
        /// Closes the welcome view and invokes the given command.
        /// </summary>
        /// <typeparam name="T">The type of command that should be invoked.</typeparam>
        /// <param name="container">The IOC container that contains all the UI controls.</param>
        /// <returns>The desired command.</returns>
        public static CompositeCommand CloseWelcomeViewAndInvokeCommand<T>(IDependencyInjectionProxy container) where T : ICommand
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
