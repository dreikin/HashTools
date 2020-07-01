using ChecksumLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;

namespace Checksummer
{
    class Program
    {
        static void Main(string[] args)
        {
            var stats = new Stats();

            // Set starting locations
            var origins = new List<string>();
            if (args.Length == 0)
            {
                origins.Add(Directory.GetCurrentDirectory());
            }
            else
            {
                foreach (var arg in args)
                {
                    if (File.Exists(arg) || Directory.Exists(arg))
                    {
                        origins.Add(arg);
                    }
                    else
                    {
                        Console.WriteLine("Origin does not exist: " + arg);
                        stats.ArgNotValid.Add(arg);
                    }
                }
            }

            // Get checksums
            var csums = new List<ChecksumInfoCollection>();
            foreach (var origin in origins)
            {
                Console.WriteLine("Origin: " + origin);
                if (Directory.Exists(origin))
                {
                    var csumCollection = CreateChecksumInfoCollection(origin, OriginType.Directory);
                    var csumCollection = new ChecksumInfoCollection(Path.GetDirectoryName(Path.GetFullPath(origin)));
                    HashTree(origin, csums, stats);
                }
                else if (File.Exists(origin))
                {
                    Console.WriteLine("Origin is a file.");
                    try
                    {
                        var csumCollection = CreateChecksumInfoCollection(origin, OriginType.File);
                        var csumCollection = new ChecksumInfoCollection(Path.GetDirectoryName(Path.GetFullPath(origin)));
                        csums.Add(GetChecksumInfo(origin));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        stats.ChecksumFailure.Add("Unauthorized: " + origin);
                    }
                    catch (Exception)
                    {
                        stats.ChecksumFailure.Add("Unknown cause: " + origin);
                    }
                }
                else
                {
                    Console.WriteLine("Origin disappeared?");
                    stats.OriginWentMissing.Add(origin);
                }
            }

            // Write stats
            Console.WriteLine($"Stats:");
            Console.WriteLine($"\tInvalid Arguments: {stats.ArgNotValid.Count}");
            foreach (var item in stats.ArgNotValid.Items)
            {
                Console.WriteLine($"\t\t{item}");
            }
            Console.WriteLine($"\tOrigins that disappeared: {stats.OriginWentMissing.Count}");
            foreach (var item in stats.OriginWentMissing.Items)
            {
                Console.WriteLine($"\t\t{item}");
            }
            Console.WriteLine($"\tUnauthorized: {stats.Unauthorized.Count}");
            foreach (var item in stats.Unauthorized.Items)
            {
                Console.WriteLine($"\t\t{item}");
            }
            Console.WriteLine($"\tFiles/Directories that disappeared: {stats.Disappeared.Count}");
            foreach (var item in stats.Disappeared.Items)
            {
                Console.WriteLine($"\t\t{item}");
            }
            Console.WriteLine($"\tFiles that couldn't be checksummed: {stats.ChecksumFailure.Count}");
            foreach (var item in stats.ChecksumFailure.Items)
            {
                Console.WriteLine($"\t\t{item}");
            }
            Console.WriteLine($"Total files checksummed: {csums.Count}");

            // Serialize data
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var serialized = JsonSerializer.SerializeToUtf8Bytes(csums, options);

            // Save to file with random name.
            do
            {
                var outfilename = string.Format(@"{0}.checksums", Guid.NewGuid());
                //outfilename = string.Format(@"{0}.checksums", Path.GetRandomFileName());

                try
                {
                    using var outfile = File.Open(outfilename, FileMode.CreateNew, FileAccess.Write);
                    outfile.Write(serialized);
                    Console.WriteLine("Checksums written to: " + Path.GetFullPath(outfilename));
                    break;
                }
                catch (IOException)
                {
                    continue;
                }
            } while (true);
        }

        private static ChecksumInfoCollection CreateChecksumInfoCollection(string origin, OriginType type)
        {
            string path;
            switch (type)
            {
                case OriginType.Directory:

                    break;
                case OriginType.File:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{type} is not a valid OriginType.");
            }
            return new ChecksumInfoCollection(path);
        }

        private static void HashTree(string origin, List<ChecksumInfo> csums, Stats stats)
        {
            // List all files
            IEnumerable<string> files = null;
            try
            {
                files = Directory.EnumerateFiles(origin);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Can't enumerate files in: " + origin);
                stats.Unauthorized.Add("Can't enumerate files in: " + origin);
            }

            if (files is { }) //not-null test using property matching.  I really just want "is not null" to be a thing.
            {
                foreach (var file in files)
                {
                    Console.WriteLine("File: " + file);
                    try
                    {
                        csums.Add(GetChecksumInfo(file));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        stats.ChecksumFailure.Add("Unauthorized: " + file);
                    }
                    catch (Exception)
                    {
                        stats.ChecksumFailure.Add("Unknown cause: " + file);
                    }
                }
            }

            // List all directories
            try
            {
                var directories = Directory.EnumerateDirectories(origin);

                foreach (var directory in directories)
                {
                    Console.WriteLine("Directory: " + directory);
                    HashTree(directory, csums, stats);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Can't enumerate directories in: " + origin);
                stats.Unauthorized.Add("Can't enumerate directories in: " + origin);
            }
        }

        public static ChecksumInfo GetChecksumInfo(string filename)
        {
            var algorithm = "SHA-512";
            return new ChecksumInfo(Path.GetFullPath(filename), algorithm, HashFile(algorithm, filename));
        }
    }

    enum OriginType
    {
        Directory,
        File
    }
}
