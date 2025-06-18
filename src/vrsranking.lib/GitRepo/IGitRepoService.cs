namespace vrsranking.lib.GitRepo;

public interface IGitRepoService
{
    public Task ExportAsync(
        TextWriter tw,
        string repositoryPath, string commitId, string toPath);

    public Task InitRepoAsync(string name, bool overwrite = false);
}