using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ChecksumLib;

namespace ChecksumCompare
{
    //class ChecksumFile
    //{
    //    public string Filename;
    //    public List<ChecksumInfoCollection> ChecksumInfoCollections;

    //    public ChecksumFile(string filename, List<ChecksumInfoCollection> checksums)
    //    {
    //        Filename = filename;
    //        ChecksumInfoCollections = checksums;
    //    }
    //}

    class Program
    {
        static void Main(string[] args)
        {

            var checksumFiles = new List<ChecksumFile>();
            if (args.Length != 3)
            {
                Console.WriteLine("This command requires three arguments.  The first is an output file path, and the next two are paths to checksum files.");
                return;
            }

            var outfilename = args[0];

            var infiles = args.TakeLast(2);
            foreach (var arg in args)
            {
                if (File.Exists(arg))
                {
                    var checksumFile = Serializer.DeserializeChecksumFileFromUtf8Json(arg);
                    checksumFile.Filename = Path.GetFullPath(arg);
                    checksumFiles.Add(checksumFile);
                }
                else
                {
                    Console.WriteLine("File does not exist: " + arg);
                    Console.WriteLine("Not enough valid checksum files.  Exiting.");
                    return;
                }
            }

            Console.WriteLine("Comparing...");
            var left = checksumFiles[0];
            var right = checksumFiles[1];
            var comparisonFile = Comparer.Compare(left, right);

            var serialized = comparisonFile.SerializeToUtf8Json();

            try
            {
                using var outfile = File.Open(outfilename, FileMode.CreateNew, FileAccess.Write);
                outfile.Write(serialized);
                Console.WriteLine("Checksums written to: " + Path.GetFullPath(outfilename));
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't write to file.  Writing to console instead.");
                Console.WriteLine(serialized);
            }

            Console.WriteLine("Stats:");
            Console.WriteLine($"\tLeft Orphan Roots: {comparisonFile.ComparisonCollections.Where(cc => cc.RightRoot is null).Count()}");
            Console.WriteLine($"\tRight Orphan Roots: {comparisonFile.ComparisonCollections.Where(cc => cc.LeftRoot is null).Count()}");
            Console.WriteLine("\tLeft Orphan Files: {0}",
                comparisonFile.ComparisonCollections.Where(cc => !(cc.RightRoot is null || cc.LeftRoot is null))
                .Select(cc => cc.LeftOrphans.Count)
                .Sum());
            Console.WriteLine("\tRight Orphan Files: {0}",
                comparisonFile.ComparisonCollections.Where(cc => !(cc.RightRoot is null || cc.LeftRoot is null))
                .Select(cc => cc.RightOrphans.Count)
                .Sum());
            Console.WriteLine("\tDifferent Files: {0}",
                comparisonFile.ComparisonCollections.Where(cc => !(cc.RightRoot is null || cc.LeftRoot is null))
                .Select(cc => cc.Different.Count)
                .Sum());
            Console.WriteLine("\tSame Files: {0}",
                comparisonFile.ComparisonCollections.Where(cc => !(cc.RightRoot is null || cc.LeftRoot is null))
                .Select(cc => cc.Same.Count)
                .Sum());
        }
    }
}
