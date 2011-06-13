//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Apollo.UI.Common.Commands;
using Apollo.UI.Common.Scripting;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// Interaction logic for ScriptView.xaml.
    /// </summary>
    public partial class ScriptView : UserControl, IScriptView
    {
        /// <summary>
        /// The collection that maps a language to an AvalonEdit highlight string for that language.
        /// </summary>
        private static readonly IDictionary<ScriptLanguage, string> s_LanguageToHighlightMap
            = new SortedList<ScriptLanguage, string> 
                {
                    { ScriptLanguage.IronPython, "Python" },
                    { ScriptLanguage.IronRuby, "Ruby" },
                };

        /// <summary>
        /// The routed command used to run a script.
        /// </summary>
        private static readonly RoutedUICommand s_RunScriptCommand
            = new RoutedUICommand(
                Apollo.UI.Common.Properties.Resources.ScriptView_Command_RunScript_Text,
                Apollo.UI.Common.Properties.Resources.ScriptView_Command_RunScript_Name,
                typeof(ScriptView),
                new InputGestureCollection { new KeyGesture(Key.F5) });

        /// <summary>
        /// The routed command used to cancel a running script.
        /// </summary>
        private static readonly RoutedUICommand s_CancelRunScriptCommand
            = new RoutedUICommand(
                Apollo.UI.Common.Properties.Resources.ScriptView_Command_CancelRunScript_Text,
                Apollo.UI.Common.Properties.Resources.ScriptView_Command_CancelRunScript_Name,
                typeof(ScriptView),
                new InputGestureCollection { new KeyGesture(Key.F5, ModifierKeys.Shift) });

        /// <summary>
        /// The routed command used to clear the output window.
        /// </summary>
        private static readonly RoutedUICommand s_ClearOutputTextCommand = new RoutedUICommand();

        /// <summary>
        /// Describes the state of the running script.
        /// </summary>
        private ScriptRunInformation m_ScriptRunInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptView"/> class.
        /// </summary>
        public ScriptView()
        {
            InitializeComponent();
            InitializeEditor();

            // Bind the run command
            {
                var cb = new CommandBinding(s_RunScriptCommand, CommandRunScriptExecuted, CommandRunScriptCanExecute);
                CommandBindings.Add(cb);
                runButton.Command = s_RunScriptCommand;
            }

            // Bind the cancel command
            {
                var cb = new CommandBinding(s_CancelRunScriptCommand, CommandCancelRunScriptExecuted, CommandCancelRunScriptCanExecute);
                CommandBindings.Add(cb);
                cancelRunButton.Command = s_CancelRunScriptCommand;
            }

            // Bind the output view clear command
            {
                var cb = new CommandBinding(s_ClearOutputTextCommand, CommandClearOutputWindowExecute, CommandClearOutputWindowCanExecute);
                CommandBindings.Add(cb);
                clearOutputMenu.Command = s_ClearOutputTextCommand;
            }
        }

        private void InitializeEditor()
        {
            textEditor.ShowLineNumbers = true;
            textEditor.TextArea.Options = new TextEditorOptions 
                { 
                    ConvertTabsToSpaces = true,
                    CutCopyWholeLine = false,
                    EnableEmailHyperlinks = false,
                    EnableHyperlinks = true,
                    EnableRectangularSelection = true,
                    EnableTextDragDrop = true,
                    IndentationSize = 4,
                    InheritWordWrapIndentation = true,
                    RequireControlModifierForHyperlinkClick = true,
                    ShowSpaces = true,
                    ShowTabs = true,
                };
            SetHighlightLanguage();
        }

        private void SetHighlightLanguage()
        {
            var scriptDescription = (Model != null) ? Model.ScriptLanguage : null;
            var language = (scriptDescription != null) ? scriptDescription.Language : ScriptLanguage.IronPython;
            var languageTag = s_LanguageToHighlightMap[language];
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(languageTag);

            // Make sure we update the syntax highlighting
            textEditor.TextArea.TextView.Redraw();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public ScriptModel Model
        {
            get
            {
                return (ScriptModel)DataContext;
            }

            set
            {
                DataContext = value;
                SetHighlightLanguage();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandNewScriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (Model != null) ? Model.NewScriptCommand.CanExecute(null) : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandNewScriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                Model.NewScriptCommand.Execute(null);
            }
            catch (CreationOfNewScriptCancelledException)
            {
                return;
            }

            textEditor.Clear();
            SetHighlightLanguage();
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandOpenScriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (Model != null) ? Model.OpenScriptCommand.CanExecute(null) : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandOpenScriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                Model.OpenScriptCommand.Execute(null);
            }
            catch (LoadingOfScriptCancelledException)
            {
                return;
            }

            textEditor.Load(Model.ScriptFile);
            SetHighlightLanguage();
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandRunScriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (Model != null) ? Model.RunCommand.CanExecute(null) : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandRunScriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput += new EventHandler<ScriptOutputEventArgs>(OnScriptOutput);

            m_ScriptRunInfo = new ScriptRunInformation
            {
                Language = Model.ScriptLanguage.Language,
                Script = textEditor.Text,
                ScriptOutput = pipe,
            };
            Model.RunCommand.Execute(m_ScriptRunInfo);
            m_ScriptRunInfo.ScriptRunningTask.ContinueWith(
                t => 
                {
                    if (t.Status == TaskStatus.Faulted)
                    {
                        WriteToOutputWindow(t.Exception.ToString());
                    }

                    m_ScriptRunInfo.ScriptOutput.OnScriptOutput += new EventHandler<ScriptOutputEventArgs>(OnScriptOutput);
                    m_ScriptRunInfo = null;
                });
        }

        private void OnScriptOutput(object sender, ScriptOutputEventArgs args)
        {
            WriteToOutputWindow(args.Text);
        }

        private void WriteToOutputWindow(string text)
        {
            Action<string> insertText = 
                s => 
                {
                    outputTextBox.Document.Insert(outputTextBox.Document.TextLength, Environment.NewLine);
                    outputTextBox.Document.Insert(outputTextBox.Document.TextLength, s);
                };

            if (Dispatcher.CheckAccess())
            {
                insertText(text);
            }
            else 
            {
                Dispatcher.Invoke(insertText, text);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandCancelRunScriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (Model != null) ? Model.CancelRunCommand.CanExecute(null) : false;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandCancelRunScriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                Model.CancelRunCommand.Execute(m_ScriptRunInfo);
                try
                {
                    m_ScriptRunInfo.ScriptRunningTask.Wait();
                }
                catch (AggregateException exception)
                {
                    WriteToOutputWindow(exception.ToString());
                }
            }
            finally
            {
                m_ScriptRunInfo.ScriptOutput.OnScriptOutput -= new EventHandler<ScriptOutputEventArgs>(OnScriptOutput);
                m_ScriptRunInfo = null;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a CanExecute event so we probably want to preserve the semantics.")]
        private void CommandClearOutputWindowCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This is really a Execute event so we probably want to preserve the semantics.")]
        private void CommandClearOutputWindowExecute(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            outputTextBox.Clear();
        }
    }
}
