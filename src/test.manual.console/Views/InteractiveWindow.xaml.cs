//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Test.Manual.Console.Models;

namespace Test.Manual.Console.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    internal partial class InteractiveWindow : Window, IInteractiveWindow
    {
        /// <summary>
        /// The command used to connect to a given endpoint.
        /// </summary>
        public static readonly RoutedCommand ConnectCommand = new RoutedCommand();

        /// <summary>
        /// The command used to send an Echo message to a given endpoint.
        /// </summary>
        public static readonly RoutedCommand SendMessageCommand = new RoutedCommand();

        /// <summary>
        /// The context that will be used to exit the application.
        /// </summary>
        private readonly System.Windows.Forms.ApplicationContext m_Context;

        /// <summary>
        /// The object that forwards communication commands onto the real
        /// communication layer.
        /// </summary>
        private readonly IHandleCommunication m_Communicator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveWindow"/> class.
        /// </summary>
        public InteractiveWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveWindow"/> class.
        /// </summary>
        /// <param name="context">The context that is use to terminate the application.</param>
        /// <param name="communicator">The object that forwards communication commands onto the real communicator.</param>
        public InteractiveWindow(System.Windows.Forms.ApplicationContext context, IHandleCommunication communicator)
            : this()
        {
            m_Context = context;
            m_Communicator = communicator;
        }

        /// <summary>
        /// Gets or sets the connection state information for the application.
        /// </summary>
        public ConnectionViewModel ConnectionState
        {
            get
            {
                return (ConnectionViewModel)DataContext;
            }
            
            set
            {
                DataContext = value;
            }
        }

        private void OnExecuteConnectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var model = e.Parameter as ConnectionInformationViewModel;
            m_Communicator.ConnectTo(model.Id);
        }

        private void OnCanExecuteConnectCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var model = e.Parameter as ConnectionInformationViewModel;
            e.Handled = true;
            e.CanExecute = (model != null) && m_Communicator.IsConnected && m_Communicator.IsEndpointKnown(model.Id);
        }

        private void OnExecuteSendMessageCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var model = e.Parameter as Tuple<string, ConnectionInformationViewModel>;
            m_Communicator.SendEchoMessageTo(model.Item2.Id, model.Item1);
        }

        private void OnCanExecuteSendMessageCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var model = e.Parameter as Tuple<string, ConnectionInformationViewModel>;
            e.Handled = true;
            e.CanExecute = (model != null) && (model.Item2 != null) && m_Communicator.IsConnected && m_Communicator.IsEndpointKnown(model.Item2.Id);
        }

        /// <summary>
        /// Handles the window closing event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void HandleWindowClosing(object sender, CancelEventArgs e)
        {
            m_Communicator.Close();

            e.Cancel = false;
            m_Context.ExitThread();
        }
    }
}
