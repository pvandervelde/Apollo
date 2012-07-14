//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apollo.UI.Common.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for SchedulingView.xaml.
    /// </summary>
    public partial class SchedulingView : UserControl, ISchedulingView
    {
        /// <summary>
        /// The routed command used to create a new schedule.
        /// </summary>
        private static readonly RoutedCommand s_NewScheduleCommand = new RoutedCommand();

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingView"/> class.
        /// </summary>
        public SchedulingView()
        {
            InitializeComponent();

            // Bind the new schedule command
            {
                var cb = new CommandBinding(s_NewScheduleCommand, CommandNewScheduleExecuted, CommandNewScheduleCanExecute);
                CommandBindings.Add(cb);
                addScheduleButton.Command = s_NewScheduleCommand;
            }
        }

        /// <summary>
        /// Gets or sets the model for the view.
        /// </summary>
        public SchedulingModel Model
        {
            get
            {
                return (SchedulingModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandNewScheduleCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model != null ? Model.AddScheduleCommand.CanExecute(null) : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandNewScheduleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Model.AddScheduleCommand.Execute(null);
        }
    }
}
