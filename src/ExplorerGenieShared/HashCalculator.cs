// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace ExplorerGenieShared
{
    /// <summary>
    /// Can calculate file hashes.
    /// </summary>
    public class HashCalculator
    {
        /// <summary>
        /// Calculates a list of hashes for the given file.
        /// </summary>
        /// <param name="filePath">Full path to the file.</param>
        /// <returns>List of calculated hashes.</returns>
        public static List<HashResult> CalculateHashes(string filePath)
        {
            List<HashResult> result = new List<HashResult>();

            using (MemoryStream fileContent = new MemoryStream())
            {
                // Read the file once and release the lock immediately
                using (Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(fileContent);
                }

                string hexEncodedHash;
                using (MD5 hasher = MD5.Create())
                {
                    hexEncodedHash = CalculateHash(fileContent, hasher);
                }
                result.Add(new HashResult("MD5", hexEncodedHash));

                using (SHA1Managed hasher = new SHA1Managed())
                {
                    hexEncodedHash = CalculateHash(fileContent, hasher);
                }
                result.Add(new HashResult("SHA-1", hexEncodedHash));

                using (SHA256Managed hasher = new SHA256Managed())
                {
                    hexEncodedHash = CalculateHash(fileContent, hasher);
                }
                result.Add(new HashResult("SHA-256", hexEncodedHash));

                using (SHA384Managed hasher = new SHA384Managed())
                {
                    hexEncodedHash = CalculateHash(fileContent, hasher);
                }
                result.Add(new HashResult("SHA-384", hexEncodedHash));

                using (SHA512Managed hasher = new SHA512Managed())
                {
                    hexEncodedHash = CalculateHash(fileContent, hasher);
                }
                result.Add(new HashResult("SHA-512", hexEncodedHash));

                using (RIPEMD160Managed hasher = new RIPEMD160Managed())
                {
                    hexEncodedHash = CalculateHash(fileContent, hasher);
                }
                result.Add(new HashResult("RIPEMD-160", hexEncodedHash));
            }
            return result;
        }

        private static string CalculateHash(Stream fileContent, HashAlgorithm hasher)
        {
            fileContent.Position = 0;
            byte[] hashBytes = hasher.ComputeHash(fileContent);
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        /// <summary>
        /// Result of a hash calculation.
        /// </summary>
        public class HashResult
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HashResult"/> class.
            /// </summary>
            /// <param name="hashAlgorithm">Sets the <see cref="HashAlgorithm"/> property.</param>
            /// <param name="hashValue">Sets the <see cref="HashValue"/> property.</param>
            public HashResult(string hashAlgorithm, string hashValue)
            {
                HashAlgorithm = hashAlgorithm;
                HashValue = hashValue;
            }

            /// <summary>Gets the name of the hash algorithm.</summary>
            public string HashAlgorithm { get; }

            /// <summary>Gets sets the calculated and encoded hash value.</summary>
            public string HashValue { get; }
        }
    }
}
