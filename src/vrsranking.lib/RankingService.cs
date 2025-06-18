using GitReader;
using GitReader.Structures;
using vrsranking.lib.GitLog;
using vrsranking.lib.GitRepo;

namespace vrsranking.lib;

public class RankingService
{
    public SortedCommitMap SortedCommit = new SortedCommitMap(new List<Commit>());
    
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
    }

    public async Task CheckHashFolderAsync()
    {
        // TODO:
        // loop through all the folders, 
        // each folder is a has
        // if a hash folder doesn't exist
        // use that hash and export
    }
    public async Task ExportHasAsync()
    {
        string[] subdirs = Directory.GetDirectories(Paths.ExportedFolders);
        
        foreach (var item in SortedCommit.dict)
        {
            foreach (var commit in item.Value)
            {
                if (!trackedHashes.Contains(commit.Hash))
                {
                    Console.WriteLine("New commit: " + commit.Hash);
                    trackedHashes.Add(commit.Hash);
                    
                    // TODO then run service to export the hash!
                }
                else
                {
                    Console.WriteLine("Already processed: " + commit.Hash);
                }
            }
        }
    }
}