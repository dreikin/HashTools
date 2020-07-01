using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ChecksumLib;

namespace ChecksumCompare
{
    class Program
    {
        static void Main(string[] args)
        {

            var csums = new Dictionary<string, List<ChecksumInfo>>();
            if (args.Length == 0)
            {
                Console.WriteLine("Please add at least two checksum files to the commandline.");
                return;
            }
            else
            {
                foreach (var arg in args)
                {
                    if (File.Exists(arg))
                    {
                        csums.Add(Path.GetFullPath(arg), LoadChecksumsFile(arg));
                    }
                    else
                    {
                        Console.WriteLine("File does not exist: " + arg);
                    }
                }
            }

            var massaged = MassageChecksumsLists(csums);
        }

        private static Dictionary<string, HashSet<ChecksumInfo>> MassageChecksumsLists(Dictionary<string, List<ChecksumInfo>> csums)
        {
            var massaged = new Dictionary<string, HashSet<ChecksumInfo>>();
            foreach (var item in csums)
            {
                IEqualityComparer<ChecksumInfo> HashComparer => {

                }
                massaged.Add(item.Key, new HashSet<ChecksumInfo>(item.Value, HashComparer));
            }
            throw new NotImplementedException();
        }

        private static List<ChecksumInfo> LoadChecksumsFile(string arg)
        {
            //throw new NotImplementedException();
            var jsonUtf8Bytes = File.ReadAllBytes(arg);
            var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<List<ChecksumInfo>>(ref utf8Reader);
        }
    }
}
