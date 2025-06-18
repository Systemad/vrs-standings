using GitReader.Structures;

namespace vrsranking.lib;

public  class CommitComparer :
    IComparer<Commit>,
    IEqualityComparer<Commit>
{
    public int Compare(Commit? x, Commit? y) =>
        y!.Committer.Date.CompareTo(x!.Committer.Date); // (Descendants)

    public bool Equals(Commit? x, Commit? y) =>
        x!.Hash.Equals(y!.Hash);

    public int GetHashCode(Commit obj) =>
        obj.Hash.GetHashCode();

    public static readonly CommitComparer Instance = new CommitComparer();
}