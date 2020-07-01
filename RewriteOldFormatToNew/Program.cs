using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
//using RewriteOldFormatToNew.Formats.v1;

namespace RewriteOldFormatToNew
{
    class Program
    {
        static void Main(string[] args)
        {
            var oldCsums = new Dictionary<string, List<Formats.v1.ChecksumInfo>>();
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
                        oldCsums.Add(Path.GetFullPath(arg), LoadV1ChecksumsFile(arg));
                    }
                    else
                    {
                        Console.WriteLine("File does not exist: " + arg);
                    }
                }
            }

            var newCsums = new ChecksumLib.ChecksumInfoCollection();
        }

        private static List<Formats.v1.ChecksumInfo> LoadV1ChecksumsFile(string arg)
        {
            //throw new NotImplementedException();
            var jsonUtf8Bytes = File.ReadAllBytes(arg);
            var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<List<Formats.v1.ChecksumInfo>>(ref utf8Reader);
        }
    }
}
