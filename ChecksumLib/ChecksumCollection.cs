using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace ChecksumLib
{
    // This specifies the root path type.
    // Files are compared by name and checksum only, while
    // directories are compared by the relative paths and checksums of each contained file.
    [Serializable]
    public enum RootType
    {
        File = 0,
        Directory = 1
    }

    [Serializable]
    public class ChecksumCollection
    {
        public RootType RootType { get; set; }
        public string Root { get; set; }
        public string RootParent { get; set; }
        public List<Checksum> Checksums { get; set; } = new List<Checksum>();

        public ChecksumCollection(string rootPath, RootType rootType)
        {
            RootType = rootType;
            var fullPath = Path.GetFullPath(rootPath);
            fullPath = Path.TrimEndingDirectorySeparator(fullPath);
            RootParent = Path.GetDirectoryName(fullPath);
            Root = Path.GetFileName(fullPath);
        }

        internal ChecksumCollection() { }

        public void Add(Checksum item)
        {
            Checksums.Add(item);
        }
    }
}
