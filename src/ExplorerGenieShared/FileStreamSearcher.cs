using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Inspired by https://learn.microsoft.com/en-us/archive/msdn-magazine/2006/january/net-matters-iterating-ntfs-streams
    /// </summary>
    public class FileStreamSearcher
    {
        public static IEnumerable<FileStreamInfo> GetStreams(string fullPath, bool ignoreMainDataStream = true)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException(nameof(fullPath));

            WIN32_FIND_STREAM_DATA findStreamData = new WIN32_FIND_STREAM_DATA();
            using (SafeFindHandle handle = FindFirstStreamW(fullPath, StreamInfoLevels.FindStreamInfoStandard, findStreamData, 0))
            {
                int lastError = Marshal.GetLastWin32Error();
                if (lastError == ERROR_INVALID_PARAMETER)
                    yield break; // file system does not support ADS (not NTFS)

                if (lastError == ERROR_HANDLE_EOF)
                    yield break; // no stream found (usually directory)

                if (lastError != ERROR_SUCCESS)
                    throw new Win32Exception(lastError);

                // Handle can be invalid by directories without data stream
                if (!handle.IsInvalid)
                {
                    do
                    {
                        if (!ignoreMainDataStream || !IsMainDataStream(findStreamData.cStreamName))
                        {
                            yield return new FileStreamInfo
                            {
                                FullPath = fullPath,
                                StreamName = findStreamData.cStreamName,
                                StreamSize = findStreamData.StreamSize,
                            };
                        }
                    }
                    while (FindNextStreamW(handle, findStreamData));

                    lastError = Marshal.GetLastWin32Error();
                    if (lastError != ERROR_HANDLE_EOF)
                        throw new Win32Exception(lastError);
                }
            }
        }

        private static bool IsMainDataStream(string streamName)
        {
            return string.Equals("::$DATA", streamName, StringComparison.OrdinalIgnoreCase);
        }

        private const int ERROR_SUCCESS = 0;
        private const int ERROR_HANDLE_EOF = 38;
        private const int ERROR_INVALID_PARAMETER = 87;

        private enum StreamInfoLevels
        {
            FindStreamInfoStandard = 0
        }

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeFindHandle FindFirstStreamW(string lpFileName, StreamInfoLevels InfoLevel, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_STREAM_DATA lpFindStreamData, uint dwFlags);

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindNextStreamW(SafeFindHandle hndFindFile, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_STREAM_DATA lpFindStreamData);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class WIN32_FIND_STREAM_DATA
        {
            public long StreamSize; [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 296)]
            public string cStreamName;
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        private sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeFindHandle()
                : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                return FindClose(handle);
            }

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            private static extern bool FindClose(IntPtr handle);
        }
    }

    public class FileStreamInfo
    {
        public string FullPath { get; set; }

        public string StreamName { get; set; }

        public long StreamSize { get; set; }
    }
}
