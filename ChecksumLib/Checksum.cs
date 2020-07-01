using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChecksumLib
{
    [Serializable]
    public class Checksum
    {
        public string FullPath { get; set; }
        public string HashAlgorithm { get; set; }
        public byte[] HashBytes { get; set; }
        public string HashString => BitConverter.ToString(HashBytes).Replace("-", "").ToLowerInvariant();
        public DateTimeOffset Timestamp { get; set; }

        public Checksum(string path, string algorithm, byte[] hashBytes)
        {
            FullPath = Path.GetFullPath(path);
            HashAlgorithm = algorithm;
            HashBytes = hashBytes.ToArray();
            Timestamp = DateTimeOffset.UtcNow;
        }

        public Checksum(string fullPath, string algorithm, byte[] hashBytes, DateTimeOffset timestamp) : this(fullPath, algorithm, hashBytes)
        {
            Timestamp = timestamp;
        }

        internal Checksum() { }
    }
}
