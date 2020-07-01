using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RewriteOldFormatToNew.Formats.v1
{
    [Serializable]
    public class ChecksumInfo
    {
        public string FullPath { get; set; }
        public string ChecksumAlgorithm { get; set; }
        public byte[] ChecksumBytes { get; set; }
        public string Checksum => BitConverter.ToString(ChecksumBytes).Replace("-", "").ToLowerInvariant();
        public DateTimeOffset Timestamp { get; set; }

        public ChecksumInfo(string fullPath, string algorithm, byte[] checksumBytes)
        {
            FullPath = fullPath;
            ChecksumAlgorithm = algorithm;
            ChecksumBytes = checksumBytes.ToArray();
            Timestamp = DateTimeOffset.UtcNow;
        }

        public ChecksumInfo(string fullPath, string algorithm, byte[] checksumBytes, DateTimeOffset timestamp)
            : this(fullPath, algorithm, checksumBytes)
        {
            Timestamp = timestamp;
        }

        internal ChecksumInfo() { }
    }
}
