using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChecksumLib
{
    public class Comparer
    {
        // CAUTION: This doesn't check for duplicate roots inside a list.
        public static ComparisonFile Compare(ChecksumFile left, ChecksumFile right)
        {
            return new ComparisonFile(Compare(left.ChecksumCollections, right.ChecksumCollections));
        }

        public static List<ComparisonCollection> Compare(List<ChecksumCollection> left, List<ChecksumCollection> right)
        {
            // Create two dictionaries each keyed by their top-most element.
            var leftCCDict = new Dictionary<string, ChecksumCollection>();
            foreach (var cc in left)
            {
                leftCCDict.Add(cc.Root, cc);
            }
            var rightCCDict = new Dictionary<string, ChecksumCollection>();
            foreach (var cc in right)
            {
                rightCCDict.Add(cc.Root, cc);
            }

            // Get keys from both dictionaries and separate based on whether they're shared or not.
            var leftRoots = leftCCDict.Keys;
            var rightRoots = rightCCDict.Keys;

            var commonRoots = leftRoots.Intersect(rightRoots);
            var leftOrphanRoots = leftRoots.Except(commonRoots);
            var rightOrphanRoots = rightRoots.Except(commonRoots);

            // Add left orphans right away, no further comparison needed.
            var comparisons = new List<ComparisonCollection>();
            foreach (var root in leftOrphanRoots)
            {
                var checksumCollection = leftCCDict[root];
                var comparisonCollection = new ComparisonCollection(checksumCollection.RootType, checksumCollection.Root, checksumCollection.RootParent, null, null, root);
                foreach (var checksum in checksumCollection)
                {
                    string commonPath;
                    if (checksum.FullPath.StartsWith(Path.Join(checksumCollection.RootParent, checksumCollection.Root)))
                    {
                        commonPath = checksum.FullPath.Remove(0, checksumCollection.RootParent.Length + 1); // Add 1 for directory separator.
                    }
                    else
                    {
                        throw new InvalidDataException("Checksum.FullPath did not start with the root's parent.");
                    }

                    comparisonCollection.LeftOrphans.Add(new ComparisonItem(commonPath, checksum, null));
                }
                comparisons.Add(comparisonCollection);
            }

            // Add right orphans right away, no further comparison needed.
            foreach (var root in rightOrphanRoots)
            {
                var checksumCollection = rightCCDict[root];
                var comparisonCollection = new ComparisonCollection(checksumCollection.RootType, null, null, checksumCollection.Root, checksumCollection.RootParent, root);
                foreach (var checksum in checksumCollection)
                {
                    string commonPath;
                    if (checksum.FullPath.StartsWith(Path.Join(checksumCollection.RootParent, checksumCollection.Root)))
                    {
                        commonPath = checksum.FullPath.Remove(0, checksumCollection.RootParent.Length + 1); // Add 1 for directory separator.
                    }
                    else
                    {
                        throw new InvalidDataException("Checksum.FullPath did not start with the root.");
                    }

                    comparisonCollection.RightOrphans.Add(new ComparisonItem(commonPath, null, checksum));
                }
                comparisons.Add(comparisonCollection);
            }

            // Compare all collections with common roots.
            foreach (var root in commonRoots)
            {
                var leftCC = leftCCDict[root];
                var rightCC = rightCCDict[root];

                var leftChecksums = leftCC.ToDictionary(ci => ci.FullPath.Remove(0, leftCC.RootParent.Length + 1));
                var rightChecksums = rightCC.ToDictionary(ci => ci.FullPath.Remove(0, rightCC.RootParent.Length + 1));

                // Sort orphan and shared paths.
                var leftPaths = leftChecksums.Keys;
                var rightPaths = rightChecksums.Keys;

                var comparable = leftPaths.Intersect(rightPaths);
                var leftOrphans = leftPaths.Except(rightPaths);
                var rightOrphans = rightPaths.Except(leftPaths);

                // Create comparison collection.
                var cc = new ComparisonCollection(leftCC.RootType, leftCC.Root, leftCC.RootParent, rightCC.Root, rightCC.RootParent, root);

                // Add left orphans.
                foreach (var path in leftOrphans)
                {
                    cc.LeftOrphans.Add(new ComparisonItem(path, leftChecksums[path], null));
                }

                // Add right orphans.
                foreach (var path in rightOrphans)
                {
                    cc.RightOrphans.Add(new ComparisonItem(path, null, rightChecksums[path]));
                }

                // Sort comparables.
                foreach (var path in comparable)
                {
                    var leftChecksum = leftChecksums[path];
                    var rightChecksum = rightChecksums[path];

                    if (leftChecksum.HashAlgorithm == rightChecksum.HashAlgorithm &&
                        leftChecksum.HashBytes == rightChecksum.HashBytes)
                    {
                        cc.Same.Add(new ComparisonItem(path, leftChecksum, rightChecksum));
                    }
                    else
                    {
                        cc.Different.Add(new ComparisonItem(path, leftChecksum, rightChecksum));
                    }
                }

                // Add to comparison collections list.
                comparisons.Add(cc);
            }

            return comparisons;
        }
    }
}
