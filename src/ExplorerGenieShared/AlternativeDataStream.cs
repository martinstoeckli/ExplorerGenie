using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Inspired by https://learn.microsoft.com/en-us/archive/msdn-magazine/2006/january/net-matters-iterating-ntfs-streams
    /// </summary>
    public class AlternativeDataStream
    {
        public const string MainDataStreamName = "::$DATA";

        /// <summary>
        /// Enumerates all streams of a single file or directory.
        /// If the file system does not support ADS, or the file does not contain a stream (directory)
        /// the enumeration will be empty.
        /// </summary>
        /// <param name="fullPath">Absolute path to a file or directory.</param>
        /// <param name="ignoreMainDataStream">Doesn't return the ::$DATA main stream if set to true.</param>
        /// <returns>Enumeration of stream infos.</returns>
        /// <exception cref="ArgumentNullException">Is thrown when the <paramref name="fullPath"/> is empty or null.</exception>
        /// <exception cref="Win32Exception">Is thrown if an error occured while reading the file.</exception>
        public static IEnumerable<FileStreamInfo> EnumerateStreams(string fullPath, bool ignoreMainDataStream = true)
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

        /// <summary>
        /// Exports an alternative data stream to a file.
        /// </summary>
        /// <param name="fullPath">Source file/directory containing the file stream.</param>
        /// <param name="streamName">Name of the stream to export.</param>
        /// <param name="destFilePath">Output file path.</param>
        /// <exception cref="Win32Exception">Is thrown when an error occured.</exception>
        public static async Task SaveStreamAs(string fullPath, string streamName, string destFilePath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException(nameof(fullPath));
            if (string.IsNullOrEmpty(streamName))
                throw new ArgumentNullException(nameof(streamName));
            if (string.IsNullOrEmpty(destFilePath))
                throw new ArgumentNullException(nameof(destFilePath));

            using (SafeFileHandle inputHandle = CreateFileW(fullPath + streamName, FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero))
            {
                int lastError = Marshal.GetLastWin32Error();
                if (lastError != ERROR_SUCCESS)
                    throw new Win32Exception(lastError);

                using (FileStream inputStream = new FileStream(inputHandle, FileAccess.Read))
                using (FileStream outputStream = File.Create(destFilePath))
                {
                    await inputStream.CopyToAsync(outputStream);
                }
            }
        }

        /// <summary>
        /// Deletes an alternative data stream from a file.
        /// </summary>
        /// <param name="fullPath">Source file/directory containing the file stream.</param>
        /// <param name="streamName">Name of the stream to export.</param>
        /// <exception cref="Win32Exception">Is thrown when an error occured.</exception>
        public static void DeleteStream(string fullPath, string streamName)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException(nameof(fullPath));
            if (string.IsNullOrEmpty(streamName))
                throw new ArgumentNullException(nameof(streamName));

            DeleteFileW(fullPath + streamName);
            int lastError = Marshal.GetLastWin32Error();
            if (lastError != ERROR_SUCCESS)
                throw new Win32Exception(lastError);
        }

        private static bool IsMainDataStream(string streamName)
        {
            return string.Equals(MainDataStreamName, streamName, StringComparison.OrdinalIgnoreCase);
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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFileW(
             [MarshalAs(UnmanagedType.LPWStr)] string filename,
             [MarshalAs(UnmanagedType.U4)] FileAccess access,
             [MarshalAs(UnmanagedType.U4)] FileShare share,
             IntPtr securityAttributes,
             [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
             [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
             IntPtr templateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool DeleteFileW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

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

    /// <summary>
    /// Describes the available information about a single data stream.
    /// </summary>
    public class FileStreamInfo
    {
        public string FullPath { get; set; }

        public string StreamName { get; set; }

        public long StreamSize { get; set; }
    }
}
