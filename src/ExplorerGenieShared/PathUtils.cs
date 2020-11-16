// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Collection of helper functions to work with file paths.
    /// </summary>
    internal static class PathUtils
    {
        /// <summary>
        /// Gets the UNC path, if the locale drive of <paramref name="filename"/> points to a UNC
        /// path.
        /// </summary>
        /// <param name="filename">Absolut file path, which may contain a locale drive pointing
        /// to an UNC path.</param>
        /// <returns>The expanded UNC path, or the original filename if it could not be expanded.</returns>
        public static string ExpandUncFilename(string filename)
        {
            string result = filename;

            // Ask for the necessary size of the buffer.
            // IntPtr.Zero is not accepted by the api, even when nothing will be written to it.
            int apiResult;
            int bufferSize = 0;
            using (var zeroBuffer = new Win32Api.SafeAllocCoTaskMem(0))
            {
                apiResult = Win32Api.WNetGetUniversalNameW(filename, Win32Api.UNIVERSAL_NAME_INFO_LEVEL, zeroBuffer.Pointer, ref bufferSize);
            }

            if (apiResult == Win32Api.ERROR_MORE_DATA)
            {
                using (var buffer = new Win32Api.SafeAllocCoTaskMem(bufferSize))
                {
                    apiResult = Win32Api.WNetGetUniversalNameW(filename, Win32Api.UNIVERSAL_NAME_INFO_LEVEL, buffer.Pointer, ref bufferSize);
                    if (apiResult == Win32Api.NOERROR)
                    {
                        Win32Api._UNIVERSAL_NAME_INFOW remoteNameInfo = (Win32Api._UNIVERSAL_NAME_INFOW)Marshal.PtrToStructure(buffer.Pointer, typeof(Win32Api._UNIVERSAL_NAME_INFOW));
                        result = Marshal.PtrToStringUni(remoteNameInfo.lpUniversalName);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a file path to its URI form.
        /// <example>
        /// C:\Temp\mit Leerschlag ➽ file:///C:/Temp/mit%20Leerschlag
        /// </example>
        /// </summary>
        /// <param name="filename">Absolute file path to convert. This file doesn't need to exist.</param>
        /// <returns>Uri of the file.</returns>
        public static string ConvertToUri(string filename)
        {
            try
            {
                Uri uriPath = new Uri(filename);
                return uriPath.AbsoluteUri;
            }
            catch
            {
                return filename;
            }
        }

        /// <summary>
        /// Escapes a file path, so it can be used in C/C++ code.
        /// <example>
        /// C:\Temp\mit Leerschlag ➽ C:\\Temp\\mit Leerschlag
        /// </example>
        /// </summary>
        /// <param name="filename">Absolute file path to convert. This file doesn't need to exist.</param>
        /// <returns>Escaped file path.</returns>
        public static string ConvertToC(string filename)
        {
            return filename.Replace(@"\", @"\\");
        }
    }
}
