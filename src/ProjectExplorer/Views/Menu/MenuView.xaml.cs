//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Input;

namespace Apollo.ProjectExplorer.Views.Menu
{
    /// <summary>
    /// A view for a top level menu.
    /// </summary>
    internal partial class MenuView : IMenuView
    {
        /// <summary>
        /// The routed command used to create a new project.
        /// </summary>
        private static readonly RoutedCommand s_NewProjectCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to load a project.
        /// </summary>
        private static readonly RoutedCommand s_OpenProjectCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to save the current project.
        /// </summary>
        private static readonly RoutedCommand s_SaveProjectCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to close the current project.
        /// </summary>
        private static readonly RoutedCommand s_CloseProjectCommand = new RoutedCommand();

        /// <summary>
        /// The routed command used to exit the application.
        /// </summary>
        private static readonly RoutedCommand s_ExitCommand = new RoutedCommand();

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

            // Bind the new project command
            {
                var cb = new CommandBinding(s_NewProjectCommand, CommandNewProjectExecuted, CommandNewProjectCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_NewProjectCommand, new KeyGesture(Key.N, ModifierKeys.Control)));

                miFileNewProject.Command = s_NewProjectCommand;
            }

            // Bind the load project command
            {
                var cb = new CommandBinding(s_OpenProjectCommand, CommandLoadProjectExecuted, CommandLoadProjectCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_OpenProjectCommand, new KeyGesture(Key.O, ModifierKeys.Control)));

                miFileOpenProject.Command = s_OpenProjectCommand;
            }

            // Bind the save project command
            {
                var cb = new CommandBinding(s_SaveProjectCommand, CommandSaveProjectExecuted, CommandSaveProjectCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_SaveProjectCommand, new KeyGesture(Key.O, ModifierKeys.Control)));

                miFileSaveProject.Command = s_SaveProjectCommand;
            }

            // Bind the close project command
            {
                var cb = new CommandBinding(s_CloseProjectCommand, CommandCloseProjectExecuted, CommandCloseProjectCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_CloseProjectCommand, new KeyGesture(Key.O, ModifierKeys.Control)));

                miFileCloseProject.Command = s_CloseProjectCommand;
            }

            // Bind the exit command
            {
                var cb = new CommandBinding(s_ExitCommand, CommandExitExecuted, CommandExitCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_ExitCommand, new KeyGesture(Key.F4, ModifierKeys.Alt)));

                miFileExit.Command = s_ExitCommand;
            }

            // Bind the About command
            {
                var cb = new CommandBinding(s_AboutCommand, CommandAboutExecuted, CommandAboutCanExecute);
                CommandBindings.Add(cb);

                InputBindings.Add(new InputBinding(s_AboutCommand, new KeyGesture(Key.F1, ModifierKeys.Alt)));

                miHelpAbout.Command = s_AboutCommand;
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
                return (MenuModel) DataContext; 
            }

            set 
            { 
                DataContext = value; 
            }
        }

        private void CommandNewProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.NewProjectCommand.CanExecute(null);
        }

        private void CommandNewProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.NewProjectCommand.Execute(null);
        }

        private void CommandLoadProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.OpenProjectCommand.CanExecute(null);
        }

        private void CommandLoadProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.OpenProjectCommand.Execute(null);
        }

        private void CommandSaveProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.SaveProjectCommand.CanExecute(null);
        }

        private void CommandSaveProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.SaveProjectCommand.Execute(null);
        }

        private void CommandCloseProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.CloseProjectCommand.CanExecute(null);
        }

        private void CommandCloseProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.CloseProjectCommand.Execute(null);
        }

        private void CommandExitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.ExitCommand.CanExecute(null);
        }

        private void CommandExitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.ExitCommand.Execute(null);
        }

        private void CommandAboutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.AboutCommand.CanExecute(null);
        }

        private void CommandAboutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.AboutCommand.Execute(null);
        }
    }
}