// Copyright (c) P. van der Velde. All right reserved
// Code based on the book: Windows Presentation Foundation Unleased
// Author: Adam Nathan

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Apollo.Ui.Common.Misc
{
    #region Structures for DLL import

    /// <summary>
    /// Defines the margins for the glass window.
    /// </summary>
    /// <remarks>
    /// In order to define a window made completely out of glass set all
    /// margins to -1.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct GlassMargins
    {
        /// <summary>
        /// Creates a new instance of the <c>GlassMargins</c> structure.
        /// </summary>
        /// <param name="t"></param>
        public GlassMargins(Thickness t)
        {
            Left = (int)t.Left;
            Right = (int)t.Right;
            Top = (int)t.Top;
            Bottom = (int)t.Bottom;
        }

        /// <summary>
        /// Stores the value for left width margin.
        /// </summary>
        public int Left;
        /// <summary>
        /// Stores the value for the right width margin.
        /// </summary>
        public int Right;
        /// <summary>
        /// Stores the value for the top width margin.
        /// </summary>
        public int Top;
        /// <summary>
        /// Stores the value for the bottom width margin.
        /// </summary>
        public int Bottom;
    };

    #endregion

    /// <summary>
    /// Defines the attached properties for a Windows Vista Aero (aka glass) effect.
    /// </summary>
    public sealed class GlassEffect
    {
        /// <summary>
        /// Extends the window frame behind the client area. 
        /// </summary>
        /// <param name="hwnd">The handle to the window for which the frame is extended.</param>
        /// <param name="pMarInset">The margins structure that describes the margins to use when extending the frame.</param>
        /// <returns>S_OK if succesfull; otherwise returns an error code.</returns>
        [DllImport("DwmApi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref GlassMargins pMarInset);
        

        /// <summary>
        /// Returns a value indicating if Aero is enabled. 
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if Aero is enabled; otherwise <see langword="false"/>.
        /// </returns>
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// Extends the glass on the specific window with the specific margins. 
        /// </summary>
        /// <param name="window">The window which has to be turned into glass.</param>
        /// <param name="margin">
        /// The extends of the glass.To get a completely
        /// glass window provide a thickness of -1 on all margins.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the extention works; otherwise <see langword="false" />.
        /// </returns>
        public static bool ExtendGlassFrame(Window window, Thickness margin)
        {
            if (!DwmIsCompositionEnabled())
            {
                return false;
            }

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException("The window must be shown before extending the glass.");

            // Set the background to transparent from both the WPF and Win32 perspectives
            window.Background = Brushes.Transparent;
            HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

            GlassMargins margins = new GlassMargins(margin);
            return (DwmExtendFrameIntoClientArea(hwnd, ref margins) == Constants.HResult.S_OK);
        }
    }
}