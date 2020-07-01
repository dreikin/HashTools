using System;
using System.Collections.Generic;
using System.Text;

namespace ChecksumLib
{
    [Serializable]
    public class ComparisonFile
    {
        public string FileType => "HashTools.ChecksumLib.ComparisonFile";
        public string Version => "2.0";
        public List<ComparisonCollection> ComparisonCollections { get; protected set; } = new List<ComparisonCollection>();

        public ComparisonFile(List<ComparisonCollection> comparisonCollections)
        {
            ComparisonCollections = comparisonCollections;
        }
    }
}
