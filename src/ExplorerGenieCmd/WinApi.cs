// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ExplorerGenieCmd
{
    /// <summary>
    /// Windows API imports, which are not (yet) part of the Dotnet version.
    /// </summary>
    internal class WinApi
    {
        [Flags]
        public enum SYMBOLIC_LINK_FLAG
        {
            File = 0,
            Directory = 1,
            AllowUnprivilegedCreate = 2
        }

        public static void CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SYMBOLIC_LINK_FLAG dwFlags)
        {
            bool success = CreateSymbolicLinkInternal(lpSymlinkFileName, lpTargetFileName, dwFlags);
            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [DllImport("Kernel32.dll", EntryPoint = "CreateSymbolicLinkW", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool CreateSymbolicLinkInternal(string lpSymlinkFileName, string lpTargetFileName, SYMBOLIC_LINK_FLAG dwFlags);
    }
}
