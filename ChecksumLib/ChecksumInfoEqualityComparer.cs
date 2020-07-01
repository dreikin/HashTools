using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ChecksumLib
{
    // Need to check for algorithm name variants.
    class ChecksumInfoEqualityComparer : IEqualityComparer<ChecksumInfo>
    {
        public bool Equals([AllowNull] ChecksumInfo x, [AllowNull] ChecksumInfo y)
        {
            if (x is null || y is null)
            {
                return false;
            }

            if (x.ChecksumBytes == y.ChecksumBytes &&
                x.ChecksumAlgorithm == y.ChecksumAlgorithm)
            {
                var xPath = x.FullPath;
                var yPath = y.FullPath;

                if (Path.IsPathFullyQualified(xPath) && Path.IsPathFullyQualified(yPath))
                {
                    // Check roots.  Remove if different, then compare.
                    var xRoot = Path.GetPathRoot(xPath);
                    var yRoot = Path.GetPathRoot(yPath);
                    if (xRoot == yRoot)
                    {
                        return xPath == yPath;
                    }
                }
                else if (Path.IsPathFullyQualified(xPath))
                {
                    // Check if yPath is subset of xPath.
                    return xPath.EndsWith(yPath);
                }
                else if (Path.IsPathFullyQualified(yPath))
                {
                    // Check if xPath is subset of yPath.
                    return yPath.EndsWith(xPath);
                }
                else
                {
                    // Check if paths are equal.
                    return xPath == yPath;
                }
            }
            else
            {
                return false;
            }
            throw new NotImplementedException();
        }

        public int GetHashCode([DisallowNull] ChecksumInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}
