using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ChecksumLib
{
    [Serializable]
    public class ChecksumFile
    {
        public string FileType => "HashTools.ChecksumLib.ChecksumFile";
        public string Version => "2.0";
        public List<ChecksumCollection> ChecksumCollections { get;  set; } = new List<ChecksumCollection>();
        public Stats Stats { get; set; } = new Stats();

        public ChecksumFile() { }
        public ChecksumFile(List<ChecksumCollection> checksumCollections, Stats stats = null)
        {
            ChecksumCollections = checksumCollections;

            if (stats is { }) //not-null test using property matching.  I really just want "is not null" to be a thing.
            {
                Stats = stats;
            }
        }

        // This is used for source tracking while working with ChecksumFiles and is not persisted.
        [NonSerialized]
        public string Filename;
    }
}
