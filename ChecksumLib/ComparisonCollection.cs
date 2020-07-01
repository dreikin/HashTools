using System;
using System.Collections.Generic;

namespace ChecksumLib
{
    [Serializable]
    public class ComparisonItem
    {
        public string CommonPath { get; protected set; }
        public string HashAlgorithm { get; protected set; }
        public Checksum Left { get; protected set; }
        public Checksum Right { get; protected set; }

        public ComparisonItem(string commonPath, Checksum left, Checksum right)
        {
            CommonPath = commonPath;
            HashAlgorithm = left.HashAlgorithm;
            Left = left;
            Right = right;
        }
    }

    [Serializable]
    public class ComparisonCollection
    {
        public RootType RootType { get; protected set; }
        public string LeftRoot { get; protected set; }
        public string LeftParent { get; protected set; }
        public string RightRoot { get; protected set; }
        public string RightParent { get; protected set; }
        public string CommonRoot { get; protected set; }

        public List<ComparisonItem> Same { get; protected set; } = new List<ComparisonItem>();
        public List<ComparisonItem> Different { get; protected set; } = new List<ComparisonItem>();
        public List<ComparisonItem> LeftOrphans { get; protected set; } = new List<ComparisonItem>();
        public List<ComparisonItem> RightOrphans { get; protected set; } = new List<ComparisonItem>();

        public ComparisonCollection(RootType rootType, string leftRoot, string leftParent, string rightRoot, string rightParent, string commonRoot)
        {
            RootType = rootType;
            LeftRoot = leftRoot;
            LeftParent = leftParent;
            RightRoot = rightRoot;
            RightParent = rightParent;
            CommonRoot = commonRoot;
        }
    }
}