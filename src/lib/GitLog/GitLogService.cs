using System.Text;
using GitReader;
using GitReader.Structures;

namespace lib.GitLog;

public interface IGitLogService
{
    public Task<HashSet<Commit>> WriteLogAsync(
        TextWriter tw,
        string repositoryPath);
}
public class GitLogService : IGitLogService
{
    public static void WriteMessage(
        TextWriter tw,
        string message)
    {
        var lines = message.Split('\n');
        var blankLines = 0;

        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                blankLines++;
                continue;
            }

            while (blankLines >= 1)
            {
                tw.WriteLine("    ");
                blankLines--;
            }

            // Expand tabs (ts = 8)
            var sb = new StringBuilder();
            for (var index = 0; index < line.Length; index++)
            {
                if (line[index] == '\t')
                {
                    var remains = 8 - index % 8;
                    while (remains > 0)
                    {
                        sb.Append(' ');
                        remains--;
                    }
                }
                else
                {
                    sb.Append(line[index]);
                }
            }

            tw.WriteLine("    " + sb);
        }
    }

    // Outputs the specified `Commit` object in a format similar to the `git log` command.
    // Returns the parent commit group referenced internally.
    public static async Task<Commit[]> WriteLogAsync(
        TextWriter tw,
        Commit commit,
        Branch? head,
        CancellationToken ct)
    {
        // It takes a small amount of time to retrieve the parent commit group.
        // It's a HACK, not a good way. But it improves processing efficiency
        // by multiply executing Task instances by awaiting them later.
        // If you divert this code, be aware of the exception that is raised until await.
        // If you leave parentsTask alone, the result of asynchronous processing will be left behind.
        var parentsTask = commit.GetParentCommitsAsync(ct);

        // Enumerate the branches and tags associated with this commit.
        var refs = string.Join(", ",
            commit.Tags.Select(t => $"tag: {t.Name}").Concat(commit.Branches.
                    // If a HEAD commit references a particular branch, this is where it is determined.
                    Select(b => head?.Name == b.Name ? $"HEAD -> {b.Name}" : b.Name))
                .OrderBy(name => name, StringComparer.Ordinal). // deterministic
                ToArray());

        if (refs.Length >= 1)
        {
            tw.WriteLine($"commit {commit.Hash} ({refs})");
        }
        else
        {
            tw.WriteLine($"commit {commit.Hash}");
        }

        // (Catch the results for asynchronous operation)
        var parents = await parentsTask;
        if (parents.Length >= 2)
        {
            var merge = string.Join(" ",
                parents.Select(p => p.Hash.ToString()).ToArray());

            tw.WriteLine($"Merge: {merge}");
        }

        // If you want to match the standard format used in Git, you can use these methods.
        tw.WriteLine($"Author: {commit.Author.ToGitAuthorString()}");
        tw.WriteLine($"Date:   {commit.Author.Date.ToGitDateString()}");

        tw.WriteLine();

        // If we are using the high-level interface,
        // the commit message is automatically split into `Subject` and `Body`.
        // Message is split into its respective properties when the message is split by an empty line.
        // If you really want to get the original message string,
        // use `GetMessage()` or use the primitive interface.
        WriteMessage(tw, commit.Subject);
        WriteMessage(tw, commit.Body);

        tw.WriteLine();

        return parents;
    }

    // Dump the entire contents of the specified local repository in `git log` format.
    public async Task<HashSet<Commit>> WriteLogAsync(
        TextWriter tw,
        string repositoryPath)
    {
        // Open the repository. This sample code uses a high-level interface.
        using var repository = await Repository.Factory.OpenStructureAsync(repositoryPath);

        // HashSet to check if the commit has already been output.
        // Initially, insert HEAD commit for all local branches and remote branches.
        var headCommits = await Task.WhenAll(
            repository.Branches.Values.Select(branch => branch.GetHeadCommitAsync()));

        var hashedCommits = new HashSet<Commit>(
            headCommits, CommitComparer.Instance);

        // SortedCommitMap to extract the next commit to be output.
        // Insert all of the above commit groups as the initial state.
        var sortedCommits = new SortedCommitMap(
            hashedCommits);

        // Insert all tag commits in hashedCommits and sortedCommits.
        // Although not common, Git tags can also be applied to trees and file objects.
        // Therefore, here we only extract tags applied to commits.
        foreach (var tag in repository.Tags.Values)
        {
            // Only commit tag.
            if (tag.Type == ObjectTypes.Commit)
            {
                var commit = await tag.GetCommitAsync();

                // Ignore commits that already exist.
                if (hashedCommits.Add(commit))
                {
                    sortedCommits.Add(commit);
                }
            }
        }

        // Also add a HEAD commit for the repository if exists.
        if (repository.Head is { } head)
        {
            var headCommit = await head.GetHeadCommitAsync();

            if (hashedCommits.Add(headCommit))
            {
                sortedCommits.Add(headCommit);
            }
        }

        // Core part of the output, continuing until all commits from sortedCommits are exhausted.
        while (sortedCommits.Contains)
        {
            // Get the commit that should be output next.
            var commit = sortedCommits.Front!;

            // Execute the output of this commit to obtain the parent commit group of this commit.
            var parents = await WriteLogAsync(tw, commit, repository.Head, default);

            // Remove this commit from sortedCommits because it is complete.
            sortedCommits.RemoveFront();

            // Add the resulting parent commits to hashedCommits and sortedCommits
            // in preparation for the next commit to be output.
            foreach (var parent in parents)
            {
                if (hashedCommits.Add(parent))
                {
                    sortedCommits.Add(parent);
                }
            }
        }
        return hashedCommits;
    }
}