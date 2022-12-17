// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace ExplorerGenieShared
{
    /// <summary>
    /// External functions of the Win32 API and helper functions.
    /// </summary>
    #pragma warning disable CS0649, IDE1006
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Imported Win32 API functions.")]
    internal static class Win32Api
    {
        public const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
        public const int REMOTE_NAME_INFO_LEVEL = 0x00000002;
        public const int ERROR_MORE_DATA = 234;
        public const int NOERROR = 0;

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern int WNetGetUniversalNameW(string lpLocalPath, [MarshalAs(UnmanagedType.U4)] int dwInfoLevel, IntPtr lpBuffer, [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);

        public struct _UNIVERSAL_NAME_INFOW
        {
            public IntPtr lpUniversalName;
        }

        public struct _REMOTE_NAME_INFOW
        {
            public IntPtr lpUniversalName;
            public IntPtr lpConnectionName;
            public IntPtr lpRemainingPath;
        }

        /// <summary>
        /// A SafeHandle for memory allocated by <see cref="Marshal.AllocCoTaskMem(int)"/>.
        /// Memory protected with this SafeHandle will be freed automatically.
        /// <example><code>
        /// using (var buffer = new Win32Api.SafeAllocCoTaskMem(1024))
        /// {
        ///     IntPtr pointer = buffer.Pointer;
        /// }
        /// </code></example>
        /// </summary>
        public class SafeAllocCoTaskMem : IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SafeAllocCoTaskMem"/> class.
            /// </summary>
            /// <param name="size">The size of the memory to allocate.</param>
            public SafeAllocCoTaskMem(int size)
            {
                Size = Math.Abs(size);
                Pointer = Marshal.AllocCoTaskMem(Size);
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="SafeAllocCoTaskMem"/> class.
            /// </summary>
            ~SafeAllocCoTaskMem()
            {
                Dispose();
            }

            /// <summary>
            /// Gets a pointer to the allocated memory.
            /// </summary>
            public IntPtr Pointer { get; private set; }

            /// <summary>
            /// Gets the size of the allocated memory.
            /// </summary>
            public int Size { get; }

            /// <inheritdoc/>
            public void Dispose()
            {
                Marshal.FreeCoTaskMem(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public static string StrFormatByteSize(long fileSize)
        {
            StringBuilder buffer = new StringBuilder(32);
            StrFormatByteSizeW(fileSize, buffer, (uint)buffer.Capacity);
            return buffer.ToString();
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern long StrFormatByteSizeW(long qdw, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszBuf, uint cchBuf);
    }
}
