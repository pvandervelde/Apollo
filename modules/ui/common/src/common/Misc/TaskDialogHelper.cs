// Code based on:
// Codeproject article: Fun with Physics (http://www.codeproject.com/KB/WPF/PhysicsFun.aspx)
// By: Sacha Barber (http://sachabarber.net/)
//     Fredrik Bornander (http://fredrik.bornander.googlepages.com/)
// The code in the article is licensed under the Code Project Open License 

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Apollo.Ui.Common.Misc
{
    /// <summary>
    /// Defines the possible dialog results for a TaskDialog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Enum values were defined in:
    /// </para>
    /// <para>
    /// Title: Windows Presentation Foundation <br />
    /// Author: Adam Nathan <br />
    /// ISBN: 0-672-32891-7 <br />
    /// </para>
    /// </remarks>
    public enum TaskDialogResult
    { 
        /// <summary>
        /// Dialog has returned an OK status.
        /// </summary>
        Ok = 1,
        /// <summary>
        /// The dialog was cancelled by the user.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// The user has clicked the retry button.
        /// </summary>
        Retry = 4,
        /// <summary>
        /// The user clicked the yes button.
        /// </summary>
        Yes = 6,
        /// <summary>
        /// The user clicked the no button.
        /// </summary>
        No = 7,
        /// <summary>
        /// The user clicked the close button.
        /// </summary>
        Close = 8,
    }

    /// <summary>
    /// Defines the possible buttons on the task dialog
    /// </summary>
    /// <remarks>
    /// <para>
    /// Enum values were defined in:
    /// </para>
    /// <para>
    /// Title: Windows Presentation Foundation <br />
    /// Author: Adam Nathan <br />
    /// ISBN: 0-672-32891-7 <br />
    /// </para>
    /// </remarks>
    [Flags]
    public enum TaskDialogButtons
    {
        /// <summary>
        /// Defines the value for the Ok button.
        /// </summary>
        Ok = 0x0001,
        /// <summary>
        /// Defines the value for the Yes button.
        /// </summary>
        Yes = 0x0002,
        /// <summary>
        /// Defines the value for the No button.
        /// </summary>
        No = 0x0004,
        /// <summary>
        /// Defines the value for the Cancel button.
        /// </summary>
        Cancel = 0x0008,
        /// <summary>
        /// Defines the value for the Retry button.
        /// </summary>
        Retry = 0x0010,
        /// <summary>
        /// Defines the value for the Close button.
        /// </summary>
        Close = 0x0020,
    }

    /// <summary>
    /// Defines the different icons that can be placed on the task dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Enum values were defined in:
    /// </para>
    /// <para>
    /// Title: Windows Presentation Foundation <br />
    /// Author: Adam Nathan <br />
    /// ISBN: 0-672-32891-7 <br />
    /// </para>
    /// </remarks>
    public enum TaskDialogIcon
    {
        /// <summary>
        /// Defines the value for the Warning icon.
        /// </summary>
        Warning = 65535,
        /// <summary>
        /// Defines the value for the Error icon.
        /// </summary>
        Error = 65534,
        /// <summary>
        /// Defines the value for the Information icon.
        /// </summary>
        Information = 65533,
        /// <summary>
        /// Defines the value for the Shield icon.
        /// </summary>
        Shield = 65532,
    }

    /// <summary>
    /// Defines methods for the creation of a Task dialog.
    /// </summary>
    /// <remarks>
    /// Note that task dialogs only exist on Windows Vista and later. On other operating systems
    /// use the MessageBox to display notices to the user.
    /// </remarks>
    public static class TaskDialog
    {
        #region DLL imports

        /// <summary>
        /// Import the TaskDialog from the Windows API.
        /// </summary>
        /// <param name="hwndParent">The handle for the parent window.</param>
        /// <param name="hInstance">The instance handle?</param>
        /// <param name="title">The title which should be displayed in the dialog.</param>
        /// <param name="mainInstruction">The text that should be displayed as the main instruction.</param>
        /// <param name="content">The text that should be displayed as the content.</param>
        /// <param name="buttons">The buttons that should be displayed.</param>
        /// <param name="icon">The icon that should be displayed.</param>
        /// <returns>A new taskdialog.</returns>
        [DllImport("comctl32.dll", PreserveSig = false, CharSet = CharSet.Unicode)]
        private static extern TaskDialogResult TaskDialog(IntPtr hwndParent, IntPtr hInstance,
            string title, string mainInstruction, string content,
            TaskDialogButtons buttons, TaskDialogIcon icon);

        #endregion

        /// <summary>
        /// Returns a value indicating if the task dialog can actually be shown. The dialog
        /// can only be shown on the operating systems Windows Vista or later.
        /// </summary>
        /// <remarks>
        /// The task dialog can only be shown on Windows Vista or later.
        /// </remarks>
        /// <returns>
        ///     <see langword="true"/> if the task dialog can be displayed; otherwise <see langword="false"/>.
        /// </returns>
        public static Boolean CanShowTaskDialog()
        {
            return OperatingSystemHelpers.Windows.IsVistaOrLater;
        }

        /// <summary>
        /// Show the task dialog for an information element. Displays the information icon and an Ok button.
        /// </summary>
        /// <param name="parent">The WPF window that is the parent for the task dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="instruction">The instruction on the dialog. This forms the actual question or remark towards the user.</param>
        /// <param name="explanation">Explains the action to be taken in a short sentence.</param>
        public static void ShowInformationDialog(Window parent, string title, string instruction, string explanation)
        {
            ShowDialog(parent, title, instruction, explanation, TaskDialogButtons.Ok, TaskDialogIcon.Information);
        }

        /// <summary>
        /// Show the task dialog for an error. Displays the Error icon and an Ok button.
        /// </summary>
        /// </summary>
        /// <param name="parent">The WPF window that is the parent for the task dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="instruction">The instruction on the dialog. This forms the actual question or remark towards the user.</param>
        /// <param name="explanation">Explains the action to be taken in a short sentence.</param>
        public static void ShowErrorDialog(Window parent, string title, string instruction, string explanation)
        {
            ShowDialog(parent, title, instruction, explanation, TaskDialogButtons.Ok, TaskDialogIcon.Error);
        }
        
        /// <summary>
        /// Show the task dialog with the specified options
        /// </summary>
        /// <param name="parent">The WPF window that is the parent for the task dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="instruction">The instruction on the dialog. This forms the actual question or remark towards the user.</param>
        /// <param name="explanation">Explains the action to be taken in a short sentence.</param>
        /// <param name="buttons">Defines the buttons that should be shown on the dialog.</param>
        /// <param name="icon">Defines the icon that should be shown on the dialog.</param>
        /// <returns>
        /// The result from the task dialog.
        /// </returns>
        public static TaskDialogResult ShowDialog(Window parent, string title, string instruction, string explanation, TaskDialogButtons buttons, TaskDialogIcon icon)
        {
            Debug.Assert(CanShowTaskDialog(), @"The operating system does not have the TaskDialog.");

            return TaskDialog(new WindowInteropHelper(parent).Handle,
                IntPtr.Zero,
                title,
                instruction,
                explanation,
                buttons,
                icon);
        }
    }
}