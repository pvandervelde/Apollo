//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Apollo.Core.Base;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Profiling;

namespace Apollo.UI.Wpf.Views.Feedback
{
    /// <summary>
    /// Interaction logic for FeedbackIconView.xaml.
    /// </summary>
    public partial class FeedbackIconView : IFeedbackView
    {
        /// <summary>
        /// The routed command used to send the error reports to the server.
        /// </summary>
        private static readonly RoutedCommand s_SendReportsCommand = new RoutedCommand();

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackIconView"/> class.
        /// </summary>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public FeedbackIconView(SystemDiagnostics systemDiagnostics)
            : this()
        {
            {
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackIconView"/> class.
        /// </summary>
        /// <remarks>
        /// Leaving this constructor separate so that the Visual Studio XAML editor
        /// can reach it. This may or may not be necessary. If it's not necessary then we
        /// should remove this constructor.
        /// </remarks>
        internal FeedbackIconView()
        {
            InitializeComponent();

            // Bind the send error reports button
            {
                var cb = new CommandBinding(s_SendReportsCommand, CommandSendReportsExecuted, CommandSendReportsCanExecute);
                CommandBindings.Add(cb);
                sendFeedbackButton.Command = s_SendReportsCommand;
            }
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public FeedbackModel Model
        {
            get
            {
                return (FeedbackModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandSendReportsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = Model != null ? Model.CanSendReport : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandSendReportsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            using (m_Diagnostics.Profiler.Measure(Globals.TimingGroup, "Sending feedback report"))
            {
                Model.SendReport();
            }
        }

        private void OnShowFeedbackButtonClick(object sender, RoutedEventArgs e)
        {
            feedbackPopup.IsOpen = true;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void ClearControls()
        {
            feedbackPopup.IsOpen = false;
            negativeFeedbackRadioButton.IsChecked = null;
            neutralFeedbackRadioButton.IsChecked = null;
            positiveFeedbackRadioButton.IsChecked = null;

            Model.Clear();
        }

        private void OnFeedbackRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(sender, negativeFeedbackRadioButton))
            {
                Model.Level = NSarrac.Framework.FeedbackLevel.Bad;
            }

            if (ReferenceEquals(sender, neutralFeedbackRadioButton))
            {
                Model.Level = NSarrac.Framework.FeedbackLevel.Neutral;
            }

            if (ReferenceEquals(sender, positiveFeedbackRadioButton))
            {
                Model.Level = NSarrac.Framework.FeedbackLevel.Good;
            }
        }

        private void OnFeedbackPopupClosed(object sender, EventArgs e)
        {
            ClearControls();
        }
    }
}
