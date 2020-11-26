// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;

namespace ExplorerGenieShared
{
    /// <summary>
    /// This class exists because from time to time, the System.Windows.Clipboard blocks the
    /// clipboard and raises an exception with error CLIPBRD_E_CANT_OPEN. The
    /// System.Windows.Forms.Clipboard can try opening the clipboard in a loop with a delay, but
    /// the problem still occurs.
    /// </summary>
    public static class Win32ApiClipboard
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        private const uint CF_UNICODETEXT = 13;

        /// <summary>
        /// Tries to set a text on the clipboard.
        /// </summary>
        /// <param name="text">The string to put on the clipboard.</param>
        /// <returns>Returns true if the text could be copied, otherwise false.</returns>
        public static bool TrySetText(string text)
        {
            if (!OpenClipboard(IntPtr.Zero))
                return false;
            try
            {
                EmptyClipboard();
                IntPtr hGlobal = Marshal.StringToHGlobalUni(text);
                try
                {
                    IntPtr res = SetClipboardData(CF_UNICODETEXT, hGlobal);
                    return (res != IntPtr.Zero);
                }
                finally
                {
                    Marshal.FreeHGlobal(hGlobal);
                }
            }
            finally
            {
                CloseClipboard();
            }
        }
    }
}
