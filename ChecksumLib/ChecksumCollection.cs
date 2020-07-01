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
    public enum RootType
    {
        File,
        Directory
    }

    [Serializable]
    public class ChecksumCollection : IList<Checksum>
    {
        public RootType RootType { get; protected set; }
        public string Root { get; protected set; }
        public string RootParent { get; protected set; }
        public List<Checksum> Checksums { get; protected set; } = new List<Checksum>();

        public ChecksumCollection(string rootPath, RootType rootType)
        {
            RootType = rootType;
            var fullPath = Path.GetFullPath(rootPath);
            fullPath = Path.TrimEndingDirectorySeparator(fullPath);
            RootParent = Path.GetDirectoryName(fullPath);
            Root = Path.GetFileName(fullPath);
        }

        #region IList implementation
        [JsonIgnore]
        public Checksum this[int index] { get => ((IList<Checksum>)Checksums)[index]; set => ((IList<Checksum>)Checksums)[index] = value; }

        [JsonIgnore]
        public int Count => ((ICollection<Checksum>)Checksums).Count;

        [JsonIgnore]
        public bool IsReadOnly => ((ICollection<Checksum>)Checksums).IsReadOnly;

        public void Add(Checksum item)
        {
            ((ICollection<Checksum>)Checksums).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Checksum>)Checksums).Clear();
        }

        public bool Contains(Checksum item)
        {
            return ((ICollection<Checksum>)Checksums).Contains(item);
        }

        public void CopyTo(Checksum[] array, int arrayIndex)
        {
            ((ICollection<Checksum>)Checksums).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Checksum> GetEnumerator()
        {
            return ((IEnumerable<Checksum>)Checksums).GetEnumerator();
        }

        public int IndexOf(Checksum item)
        {
            return ((IList<Checksum>)Checksums).IndexOf(item);
        }

        public void Insert(int index, Checksum item)
        {
            ((IList<Checksum>)Checksums).Insert(index, item);
        }

        public bool Remove(Checksum item)
        {
            return ((ICollection<Checksum>)Checksums).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Checksum>)Checksums).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Checksums).GetEnumerator();
        }
        #endregion IList implementation
    }
}
