using System;
using System.Collections.Generic;
using System.Text;

namespace ChecksumLib
{
    [Serializable]
    public class StatItem
    {
        public long Count { get; protected set; }
        public List<string> Items { get; protected set; } = new List<string>();

        public void Add(string item)
        {
            Count++;
            Items.Add(item);
        }
    }

    [Serializable]
    public class Stats
    {
        public StatItem ArgNotValid { get; protected set; } = new StatItem();
        public StatItem OriginWentMissing { get; protected set; } = new StatItem();
        public StatItem Unauthorized { get; protected set; } = new StatItem();
        public StatItem Disappeared { get; protected set; } = new StatItem();
        public StatItem ChecksumFailure { get; protected set; } = new StatItem();
    }
}
