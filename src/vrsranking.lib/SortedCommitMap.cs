using GitReader.Structures;

namespace vrsranking.lib;

public class SortedCommitMap
{
    public readonly SortedDictionary<Commit, LinkedList<Commit>> dict =
        new(CommitComparer.Instance);

    public SortedCommitMap(IEnumerable<Commit> commits)
    {
        foreach (var commit in commits)
        {
            this.Add(commit);
        }
    }

    public bool Contains =>
        this.dict.Count >= 1;

    public int Count =>
        this.dict.Values.Sum(commit => commit.Count);

    // One of the first commits.
    public Commit? Front =>
        this.dict.Values.FirstOrDefault()?.First!.Value;

    public bool Add(Commit commit)
    {
        if (!this.dict.TryGetValue(commit, out var commits))
        {
            commits = new();
            this.dict.Add(commit, commits);
            commits.AddLast(commit);
            return true;
        }

        foreach (var c in commits)
        {
            if (c.Hash.Equals(commit.Hash))
            {
                return false;
            }
        }

        commits.AddLast(commit);
        return true;
    }

    public bool RemoveFront()
    {
        var commit = this.Front!;
        if (this.dict.TryGetValue(commit, out var commits))
        {
            for (var index = 0; index < commits.Count; index++)
            {
                foreach (var c in commits)
                {
                    if (c.Hash.Equals(commit.Hash))
                    {
                        commits.Remove(c);
                        if (commits.Count == 0)
                        {
                            this.dict.Remove(commit);
                        }

                        return true;
                    }
                }
            }
        }

        return false;
    }
}