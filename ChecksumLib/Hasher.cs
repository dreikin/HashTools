using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace ChecksumLib
{
    public class Hasher
    {
        public static ChecksumInfo HashFile(string path, string algorithm = "SHA512")
        {
            using var hasher = HashAlgorithm.Create(algorithm);
            if (hasher is null)
            {
                throw new ArgumentException("Not a recognized hash algorithm.  See https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm.create?view=netcore-3.1#System_Security_Cryptography_HashAlgorithm_Create_System_String_ for acceptable values.");
            }

            using var stream = File.OpenRead(path);
            var timestamp = DateTimeOffset.UtcNow;
            var hash = hasher.ComputeHash(stream);
            return new ChecksumInfo(path, algorithm, hash, timestamp);
        }

        public static string HashToString(byte[] hashBytes)
        {
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
