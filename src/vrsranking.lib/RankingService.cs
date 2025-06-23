using GitReader;
using GitReader.Structures;
using vrsranking.lib.GitLog;
using vrsranking.lib.GitRepo;

namespace vrsranking.lib;

public interface IRankingService
{
    Task FetchLatestChangesAsync();
    Task ExportHashCommit();
}

public class RankingService : IRankingService
{
    public SortedCommitMap SortedCommit = new SortedCommitMap(new List<Commit>());
    public HashSet<Commit> HashedCommits = new HashSet<Commit>();

    // Task<HashSet<Commit>>
    HashSet<Hash> trackedHashes = new HashSet<Hash>();

    public IGitRepoService GitRepoService;
    public IGitLogService GitLogService;

    public RankingService(IGitRepoService gitRepoService, IGitLogService gitLogService)
    {
        GitRepoService = gitRepoService;
        GitLogService = gitLogService;
    }

    public async Task FetchLatestChangesAsync()
    {
        var commits = await GitLogService.WriteLogAsync(Console.Out, Paths.RankingsRepo);
        HashedCommits = commits;
    }

    public async Task CheckHashFolderAsync()
    {
        string[] subdirs = Directory.GetDirectories(Paths.ExportedRoot);

        // TODO:
        // loop through all the folders, 
        // each folder is a has
        // if a hash folder doesn't exist
        // use that hash and export
    }

    async Task ExportSafe(Commit hash)
    {
        await GitRepoService.ExportAsync(Console.Out, Paths.RankingsRepo, hash.Hash.ToString(),
            "Paths.ExportedRankings" + "\\" + hash.Hash.ToString());
    }

    public async Task ExportHashCommit()
    {
        await Task.WhenAll(HashedCommits.Take(5).Select(hash => ExportSafe(hash)));
        /*
        foreach (var hash in HashedCommits)
        {
            try
            {
                Console.WriteLine(hash);
                await GitRepoService.ExportAsync(Console.Out, Paths.RankingsRepo, hash.Hash.ToString(),
                    Paths.ExportedRankings + "\\" + hash.Hash.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to export commit {hash.Hash}: {e.Message}");
            }

        }
        */
    }

}