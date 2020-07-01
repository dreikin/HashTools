using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ChecksumLib
{
    public static class Serializer
    {
        public static byte[] SerializeToUtf8Json(this ChecksumFile checksumFile)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return JsonSerializer.SerializeToUtf8Bytes(checksumFile, options);
        }

        public static ChecksumFile DeserializeChecksumFileFromUtf8Json(string filename)
        {
            var jsonUtf8Bytes = File.ReadAllBytes(filename);
            return DeserializeChecksumFileFromUtf8Json(jsonUtf8Bytes);
        }

        public static ChecksumFile DeserializeChecksumFileFromUtf8Json(byte[] jsonUtf8Bytes)
        {
            var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<ChecksumFile>(ref utf8Reader);
        }

        public static byte[] SerializeToUtf8Json(this ComparisonFile comparisonFile)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return JsonSerializer.SerializeToUtf8Bytes(comparisonFile, options);
        }

        public static ComparisonFile DeserializeComparisonFileFromUtf8Json(string filename)
        {
            var jsonUtf8Bytes = File.ReadAllBytes(filename);
            return DeserializeComparisonFileFromUtf8Json(jsonUtf8Bytes);
        }

        public static ComparisonFile DeserializeComparisonFileFromUtf8Json(byte[] jsonUtf8Bytes)
        {
            var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<ComparisonFile>(ref utf8Reader);
        }

        private static byte[] SerializeToUtf8Json(this List<ChecksumCollection> checksums)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return JsonSerializer.SerializeToUtf8Bytes(checksums, options);
        }

        private static List<ChecksumCollection> DeserializeChecksumsFromUtf8Json(string filename)
        {
            var jsonUtf8Bytes = File.ReadAllBytes(filename);
            return DeserializeChecksumsFromUtf8Json(jsonUtf8Bytes);
        }

        private static List<ChecksumCollection> DeserializeChecksumsFromUtf8Json(byte[] jsonUtf8Bytes)
        {
            var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<List<ChecksumCollection>>(ref utf8Reader);
        }

        private static byte[] SerializeToUtf8Json(this List<ComparisonCollection> comparisons)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return JsonSerializer.SerializeToUtf8Bytes(comparisons, options);
        }

        private static List<ComparisonCollection> DeserializeComparisonsFromUtf8Json(string filename)
        {
            var jsonUtf8Bytes = File.ReadAllBytes(filename);
            return DeserializeComparisonsFromUtf8Json(jsonUtf8Bytes);
        }

        private static List<ComparisonCollection> DeserializeComparisonsFromUtf8Json(byte[] jsonUtf8Bytes)
        {
            var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
            return JsonSerializer.Deserialize<List<ComparisonCollection>>(ref utf8Reader);
        }
    }
}
