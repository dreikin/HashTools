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
    public class ChecksumInfoCollection : IList<ChecksumInfo>
    {
        public RootType RootType { get; protected set; }
        public string RootPath { get; protected set; }
        public List<ChecksumInfo> ChecksumInfos { get; protected set; } = new List<ChecksumInfo>();

        public Stats Stats { get; protected set; } = new Stats();

        public ChecksumInfoCollection(string rootPath, RootType rootType)
        {
            RootType = rootType;
            RootPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(rootPath));
        }

        #region IList implementation
        [JsonIgnore]
        public ChecksumInfo this[int index] { get => ((IList<ChecksumInfo>)ChecksumInfos)[index]; set => ((IList<ChecksumInfo>)ChecksumInfos)[index] = value; }

        [JsonIgnore]
        public int Count => ((ICollection<ChecksumInfo>)ChecksumInfos).Count;

        [JsonIgnore]
        public bool IsReadOnly => ((ICollection<ChecksumInfo>)ChecksumInfos).IsReadOnly;

        public void Add(ChecksumInfo item)
        {
            ((ICollection<ChecksumInfo>)ChecksumInfos).Add(item);
        }

        public void Clear()
        {
            ((ICollection<ChecksumInfo>)ChecksumInfos).Clear();
        }

        public bool Contains(ChecksumInfo item)
        {
            return ((ICollection<ChecksumInfo>)ChecksumInfos).Contains(item);
        }

        public void CopyTo(ChecksumInfo[] array, int arrayIndex)
        {
            ((ICollection<ChecksumInfo>)ChecksumInfos).CopyTo(array, arrayIndex);
        }

        public IEnumerator<ChecksumInfo> GetEnumerator()
        {
            return ((IEnumerable<ChecksumInfo>)ChecksumInfos).GetEnumerator();
        }

        public int IndexOf(ChecksumInfo item)
        {
            return ((IList<ChecksumInfo>)ChecksumInfos).IndexOf(item);
        }

        public void Insert(int index, ChecksumInfo item)
        {
            ((IList<ChecksumInfo>)ChecksumInfos).Insert(index, item);
        }

        public bool Remove(ChecksumInfo item)
        {
            return ((ICollection<ChecksumInfo>)ChecksumInfos).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ChecksumInfo>)ChecksumInfos).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)ChecksumInfos).GetEnumerator();
        }
        #endregion IList implementation
    }
}
