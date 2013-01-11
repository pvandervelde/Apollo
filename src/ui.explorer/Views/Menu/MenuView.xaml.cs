//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Apollo.Utilities;
using NManto;

namespace Apollo.UI.Explorer.Views.Menu
{
    /// <summary>
    /// A view for a top level menu.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal partial class MenuView : IMenuView
    {
        /// <summary>
        /// The routed command used to exit the application.
        /// </summary>
        private static readonly RoutedCommand s_ExitCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to undo the last action.
        /// </summary>
        private static readonly RoutedCommand s_UndoCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to redo the last undo action.
        /// </summary>
        private static readonly RoutedCommand s_RedoCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to show the start page tab.
        /// </summary>
        private static readonly RoutedCommand s_ShowStartPageCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to show the projects tab.
        /// </summary>
        private static readonly RoutedCommand s_ShowProjectsCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to show the scripts tab.
        /// </summary>
        private static readonly RoutedCommand s_ShowScriptsCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to display the about box.
        /// </summary>
        private static readonly RoutedCommand s_AboutCommand = new RoutedCommand();

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuView"/> class.
        /// </summary>
        public MenuView()
        {
            InitializeComponent();

            BindFileMenuCommands();
            BindEditMenuCommands();
            BindViewMenuCommands();
            BindHelpMenuCommands();
        }

        private void BindFileMenuCommands()
        {
            // Bind the exit command
            {
                var cb = new CommandBinding(s_ExitCommand, CommandExitExecuted, CommandExitCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_ExitCommand, new KeyGesture(Key.F4, ModifierKeys.Alt)));

                // Set the command and set the command target to the control so that we don't run into focus issues
                // as given here:
                // http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/f5de6ffc-fa03-4f08-87e9-77bbad752033
                miFileExit.Command = s_ExitCommand;
                miFileExit.CommandTarget = this;
            }
        }

        private void BindEditMenuCommands()
        {
            // Bind the undo command
            {
                var cb = new CommandBinding(s_UndoCommand, CommandUndoExecuted, CommandUndoCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_UndoCommand, new KeyGesture(Key.Z, ModifierKeys.Control)));

                // Set the command and set the command target to the control so that we don't run into focus issues
                // as given here:
                // http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/f5de6ffc-fa03-4f08-87e9-77bbad752033
                miEditUndo.Command = s_UndoCommand;
                miEditUndo.CommandTarget = this;
            }

            // Bind the redo command
            {
                var cb = new CommandBinding(s_RedoCommand, CommandRedoExecuted, CommandRedoCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_RedoCommand, new KeyGesture(Key.Y, ModifierKeys.Control)));

                // Set the command and set the command target to the control so that we don't run into focus issues
                // as given here:
                // http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/f5de6ffc-fa03-4f08-87e9-77bbad752033
                miEditRedo.Command = s_RedoCommand;
                miEditRedo.CommandTarget = this;
            }
        }

        private void BindViewMenuCommands()
        {
            // Bind the start page command
            {
                var cb = new CommandBinding(s_ShowStartPageCommand, CommandShowStartPageExecuted, CommandShowStartPageCanExecute);
                CommandBindings.Add(cb);

                miViewStartPage.Command = s_ShowStartPageCommand;
                miViewStartPage.CommandTarget = this;
            }

            // Bind the projects command
            {
                var cb = new CommandBinding(s_ShowProjectsCommand, CommandShowProjectsExecuted, CommandShowProjectsCanExecute);
                CommandBindings.Add(cb);

                miViewProjects.Command = s_ShowProjectsCommand;
                miViewProjects.CommandTarget = this;
            }

            // Bind the scripts command
            {
                var cb = new CommandBinding(s_ShowScriptsCommand, CommandShowScriptsExecuted, CommandShowScriptsCanExecute);
                CommandBindings.Add(cb);

                miViewScript.Command = s_ShowScriptsCommand;
                miViewScript.CommandTarget = this;
            }
        }

        private void BindHelpMenuCommands()
        {
            // Bind the About command
            {
                var cb = new CommandBinding(s_AboutCommand, CommandAboutExecuted, CommandAboutCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_AboutCommand, new KeyGesture(Key.F1, ModifierKeys.Alt)));

                miHelpAbout.Command = s_AboutCommand;
                miHelpAbout.CommandTarget = this;
            }
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public MenuModel Model
        {
            get
            {
                return (MenuModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandNewProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.NewProjectCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandNewProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.NewProjectCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandLoadProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.OpenProjectCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandLoadProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.OpenProjectCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandSaveProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.SaveProjectCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandSaveProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.SaveProjectCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandCloseProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.CloseProjectCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandCloseProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.CloseProjectCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandExitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.ExitCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            // We could put an timing interval around this but it may not be useful
            // given that we're about to exit from the application. This means that
            // the profiler might be destroyed halfway the timing process.
            Model.ExitCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandShowStartPageCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.ShowStartPageCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandShowStartPageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.ShowStartPageCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandShowProjectsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.ShowProjectsCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandShowProjectsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.ShowProjectsCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandShowScriptsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.ShowScriptsCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandShowScriptsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.ShowScriptsCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandAboutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.AboutCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandAboutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.AboutCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandUndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.UndoCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandUndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.UndoCommand.Execute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandRedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.RedoCommand.CanExecute(null);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandRedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.RedoCommand.Execute(null);
        }
    }
}
