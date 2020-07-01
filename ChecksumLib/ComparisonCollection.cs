using System;
using System.Collections.Generic;
using System.IO;

namespace ChecksumLib
{
    [Serializable]
    public class ComparisonItem
    {
        public string CommonPath { get; set; }
        public string HashAlgorithm { get; set; }
        public Checksum Left { get; set; }
        public Checksum Right { get; set; }

        public ComparisonItem(string commonPath, Checksum left, Checksum right)
        {
            CommonPath = commonPath;
            if (left is null && right is null)
            {
                throw new InvalidDataException("Can't compare two nulls");
            }
            else if (left is null)
            {
                HashAlgorithm = right.HashAlgorithm;
            }
            else
            {
                HashAlgorithm = left.HashAlgorithm;
            }
            Left = left;
            Right = right;
        }

        internal ComparisonItem() { }
    }

    [Serializable]
    public class ComparisonCollection
    {
        public RootType RootType { get; set; }
        public string LeftRoot { get; set; }
        public string LeftParent { get; set; }
        public string RightRoot { get; set; }
        public string RightParent { get; set; }
        public string CommonRoot { get; set; }

        public List<ComparisonItem> Same { get; set; } = new List<ComparisonItem>();
        public List<ComparisonItem> Different { get; set; } = new List<ComparisonItem>();
        public List<ComparisonItem> LeftOrphans { get; set; } = new List<ComparisonItem>();
        public List<ComparisonItem> RightOrphans { get; set; } = new List<ComparisonItem>();

        public ComparisonCollection(RootType rootType, string leftRoot, string leftParent, string rightRoot, string rightParent, string commonRoot)
        {
            RootType = rootType;
            LeftRoot = leftRoot;
            LeftParent = leftParent;
            RightRoot = rightRoot;
            RightParent = rightParent;
            CommonRoot = commonRoot;
        }

        internal ComparisonCollection() { }
    }
}