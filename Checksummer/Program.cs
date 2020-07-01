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
            var checksumFile = new ChecksumFile();
            var stats = checksumFile.Stats;

            // Set starting locations
            var roots = new List<string>();
            if (args.Length == 0)
            {
                roots.Add(Directory.GetCurrentDirectory());
            }
            else
            {
                foreach (var arg in args)
                {
                    if (File.Exists(arg) || Directory.Exists(arg))
                    {
                        roots.Add(arg);
                    }
                    else
                    {
                        Console.WriteLine("Root does not exist: " + arg);
                        stats.ArgNotValid.Add(arg);
                    }
                }
            }

            // Get checksums
            var checksumCollections = checksumFile.ChecksumCollections;
            foreach (var root in roots)
            {
                Console.WriteLine("Root: " + root);
                if (Directory.Exists(root))
                {
                    var checksumCollection = new ChecksumCollection(root, RootType.Directory);
                    HashTree(root, checksumCollection, stats);
                    checksumCollections.Add(checksumCollection);
                }
                else if (File.Exists(root))
                {
                    Console.WriteLine("Root is a file.");
                    try
                    {
                        var checksumCollection = new ChecksumCollection(root, RootType.File);
                        checksumCollection.Add(Hasher.HashFile(root));
                        checksumCollections.Add(checksumCollection);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        stats.ChecksumFailure.Add("Unauthorized: " + root);
                    }
                    catch (Exception)
                    {
                        stats.ChecksumFailure.Add("Unknown cause: " + root);
                    }
                }
                else
                {
                    Console.WriteLine("Root disappeared?");
                    stats.RootWentMissing.Add(root);
                }
            }

            // Write stats
            Console.WriteLine($"Stats:");
            Console.WriteLine($"\tInvalid Arguments: {stats.ArgNotValid.Count}");
            foreach (var item in stats.ArgNotValid.Items)
            {
                Console.WriteLine($"\t\t{item}");
            }
            Console.WriteLine($"\tRoots that disappeared: {stats.RootWentMissing.Count}");
            foreach (var item in stats.RootWentMissing.Items)
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
            Console.WriteLine($"Total files checksummed: {checksumCollections.Select(ci => ci.Checksums.Count).Sum()}");

            // Serialize data
            var serialized = checksumFile.SerializeToUtf8Json();

            // Save to file with random name.
            do
            {
                var outfilename = string.Format(@"{0}.checksums.json", Guid.NewGuid());
                //outfilename = string.Format(@"{0}.checksums", Path.GetRandomFileName());

                try
                {
                    using var outfile = File.Open(outfilename, FileMode.CreateNew, FileAccess.Write);
                    outfile.Write(serialized);
                    Console.WriteLine("ChecksumFile written to: " + Path.GetFullPath(outfilename));
                    break;
                }
                catch (IOException)
                {
                    continue;
                }
            } while (true);
        }

        private static void HashTree(string root, ChecksumCollection checksumCollection, Stats stats)
        {
            // List all files
            IEnumerable<string> files = null;
            try
            {
                files = Directory.EnumerateFiles(root);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Can't enumerate files in: " + root);
                stats.Unauthorized.Add("Can't enumerate files in: " + root);
            }

            if (files is { }) //not-null test using property matching.  I really just want "is not null" to be a thing.
            {
                foreach (var file in files)
                {
                    Console.WriteLine("File: " + file);
                    try
                    {
                        checksumCollection.Add(Hasher.HashFile(file));
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
            IEnumerable<string> directories = null;
            try
            {
                directories = Directory.EnumerateDirectories(root);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Can't enumerate directories in: " + root);
                stats.Unauthorized.Add("Can't enumerate directories in: " + root);
            }

            if (directories is { }) //not-null test using property matching.  I really just want "is not null" to be a thing.
            {
                foreach (var directory in directories)
                {
                    Console.WriteLine("Directory: " + directory);
                    HashTree(directory, checksumCollection, stats);
                }
            }
        }
    }
}
