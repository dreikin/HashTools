using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ChecksumLib
{
    public class ChecksumInfoCollectionEqualityComparer : EqualityComparer<ChecksumInfoCollection>
    {
        public override bool Equals([AllowNull] ChecksumInfoCollection x, [AllowNull] ChecksumInfoCollection y)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode([DisallowNull] ChecksumInfoCollection obj)
        {
            throw new NotImplementedException();
        }
    }
}
