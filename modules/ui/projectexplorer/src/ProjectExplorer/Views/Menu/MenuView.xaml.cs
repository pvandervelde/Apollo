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

        private void CommandAboutCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.AboutCommand.CanExecute(null);
        }

        private void CommandAboutExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.AboutCommand.Execute(null);
        }

        private void CommandExitCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model.ExitCommand.CanExecute(null);
        }

        private void CommandExitExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.ExitCommand.Execute(null);
        }
    }
}