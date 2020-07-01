using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RewriteOldFormatToNew
{
    class Program
    {
        // This program currently assumes very specific details based on what I used each version for.
        static void Main(string[] args)
        {
            var oldChecksums = new Dictionary<string, List<Formats.v1.ChecksumInfo>>();
            if (args.Length == 0)
            {
                Console.WriteLine("Please add at least one checksum file path to the commandline.");
                return;
            }
            else
            {
                foreach (var arg in args)
                {
                    if (File.Exists(arg))
                    {
                        oldChecksums.Add(Path.GetFullPath(arg), LoadV1ChecksumsFile(arg));
                    }
                    else
                    {
                        Console.WriteLine("File does not exist: " + arg);
                    }
                }
            }

            foreach (var checksumFile in oldChecksums)
            {
                var path = checksumFile.Key;
                var checksums = checksumFile.Value;
                var checksumCollectionsByRoot = new Dictionary<string, ChecksumLib.ChecksumCollection>();

                foreach (var oldChecksum in checksums)
                {
                    if (oldChecksum.ChecksumAlgorithm != "SHA-512")
                    {
                        throw new InvalidDataException("Something didn't have 'SHA-512' as the algorithm.  Actual value: " + oldChecksum);
                    }

                    // Get root path.  If it's not already a ChecksumCollection root, then add a new one to the dictionary.
                    var pathElements = oldChecksum.FullPath.Split(Path.DirectorySeparatorChar);
                    var root = Path.Combine(pathElements[0], pathElements[1]);
                    if(!checksumCollectionsByRoot.ContainsKey(root))
                    {
                        checksumCollectionsByRoot.Add(root, new ChecksumLib.ChecksumCollection(root, ChecksumLib.RootType.Directory));
                    }

                    // Add the new checksum to the appropriate collection.
                    var newChecksum = new ChecksumLib.Checksum(oldChecksum.FullPath, "SHA512", oldChecksum.ChecksumBytes, oldChecksum.Timestamp);
                    checksumCollectionsByRoot[root].Add(newChecksum);
                }

                // Convert the dictionary to a list and serialize it to a new name based off the old one.
                var newChecksumFile = new ChecksumLib.ChecksumFile(checksumCollectionsByRoot.Values.ToList());
                var serialized = ChecksumLib.Serializer.SerializeToUtf8Json(newChecksumFile);

                var filename = Path.GetFileNameWithoutExtension(path);
                var fileDirectory = Path.GetDirectoryName(path);

                var outfilename = string.Format(@"newformat-{0}.checksums.json", filename);
                outfilename = Path.Join(fileDirectory, outfilename);
                try
                {
                    using var outfile = File.Open(outfilename, FileMode.CreateNew, FileAccess.Write);
                    outfile.Write(serialized);
                    Console.WriteLine("ChecksumFile written to: " + Path.GetFullPath(outfilename));
                }
                catch (IOException)
                {
                    Console.WriteLine($"Couldn't write file: {outfilename}");
                }
            }
        }

        private static List<Formats.v1.ChecksumInfo> LoadV1ChecksumsFile(string arg)
        {
            var jsonUtf8Bytes = File.ReadAllBytes(arg);
            var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<List<Formats.v1.ChecksumInfo>>(ref utf8Reader);
        }
    }
}
