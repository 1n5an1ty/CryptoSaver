using System;
using System.Runtime.InteropServices;

namespace CryptoSaver.Core.Internals
{
    /// <summary>
    /// Class NativeMethods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Sets the window long.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <param name="dwNewLong">The dw new long.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Gets the window long.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="nIndex">Index of the n.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // http://msdn.microsoft.com/en-us/library/ms633519(VS.85).aspx
        /// <summary>
        /// Gets the window rect.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="lpRect">The lp rect.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="hParent">The h parent.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("User32.dll")]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hParent);

        #region Nested type: RECT

        // http://msdn.microsoft.com/en-us/library/a5ch4fda(VS.80).aspx
        /// <summary>
        /// Struct RECT
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// The left
            /// </summary>
            public int Left;
            /// <summary>
            /// The top
            /// </summary>
            public int Top;
            /// <summary>
            /// The right
            /// </summary>
            public int Right;
            /// <summary>
            /// The bottom
            /// </summary>
            public int Bottom;
        }

        #endregion
    }
}