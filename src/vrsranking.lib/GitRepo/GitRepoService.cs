using System.Diagnostics;
using GitReader;
using GitReader.Structures;

namespace vrsranking.lib.GitRepo;

public class GitRepoService : IGitRepoService
{
    private int _directories = 0;
    private int _files = 0;

    static async ValueTask WhenAll(IEnumerable<ValueTask> tasks)
    {
        foreach (var task in tasks.ToArray())
        {
            await task;
        }
    }

    public async Task ExportAsync(
        TextWriter tw,
        string repositoryPath, string commitId, string toPath)
    {
        if (Directory.Exists(toPath))
        {
            Directory.Delete(toPath, true);
            Directory.CreateDirectory(toPath);
        }

        var sw = Stopwatch.StartNew();

        using var repository =
            await Repository.Factory.OpenStructureAsync(repositoryPath);

        var opened = sw.Elapsed;
        tw.WriteLine($"Opened: {opened}");

        var commit = await repository.GetCommitAsync(commitId);

        var gotCommit = sw.Elapsed;
        tw.WriteLine($"Got commit: {gotCommit - opened}");

        var tree = await commit!.GetTreeRootAsync();

        var gotTree = sw.Elapsed;
        tw.WriteLine($"Got tree: {gotTree - gotCommit}");


        await ExtractTreeAsync(tree.Children, toPath);

        var extracted = sw.Elapsed;
        var e = extracted - gotTree;
        tw.WriteLine($"Extracted: {e}");

        tw.WriteLine();

        var dr = (double)_directories / (_directories + _files);
        var d = TimeSpan.FromTicks((long)(e.Ticks * dr));
        tw.WriteLine($"Directories: {_directories}, {d}");

        var fr = (double)_files / (_directories + _files);
        var f = TimeSpan.FromTicks((long)(e.Ticks * fr));
        tw.WriteLine($"Files: {_files}, {f}");
    }



    public async ValueTask ExtractBlobAsync(
        TreeBlobEntry blob, string path)
    {
        var openBlobAsync = blob.OpenBlobAsync();

        var basePath = Path.GetDirectoryName(path)!;
        while (!Directory.Exists(basePath))
        {
            try
            {
                Directory.CreateDirectory(basePath);
            }
            catch
            {
            }
        }

        var stream = await openBlobAsync;

        using var fs = new FileStream(
            path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 65536, true);
        await stream.CopyToAsync(fs);
        await fs.FlushAsync();
    }

    public ValueTask ExtractTreeAsync(TreeEntry[] entries, string basePath) =>
        WhenAll(entries.Select(entry =>
        {
            var path = Path.Combine(basePath, entry.Name);
            switch (entry)
            {
                case TreeDirectoryEntry directory:
                    Interlocked.Increment(ref _directories);
                    while (!Directory.Exists(path))
                    {
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch
                        {
                        }
                    }

                    return ExtractTreeAsync(directory.Children, path);
                case TreeBlobEntry blob:
                    Interlocked.Increment(ref _files);
                    return ExtractBlobAsync(blob, path);
                case TreeSubModuleEntry subModule:
                    while (!Directory.Exists(path))
                    {
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch
                        {
                        }
                    }

                    return ExtractSubModule(subModule, path);
                default:
                    return default;
            }
        }));

    public async ValueTask ExtractSubModule(TreeSubModuleEntry subModule, string basePath)
    {
        using var subModuleRepository = await subModule.OpenSubModuleAsync();

        var subModuleCommit = await subModuleRepository.GetCommitAsync(subModule);
        var subModuleRootTree = await subModuleCommit!.GetTreeRootAsync();

        await ExtractTreeAsync(subModuleRootTree.Children, basePath);
    }

    void RemoveReadOnlyAttributes(string directory)
    {
        var dirInfo = new DirectoryInfo(directory);
        foreach (var file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            file.Attributes = FileAttributes.Normal;
        }
    }
    
    public async Task InitRepoAsync(string name, bool overwrite = false)
    {
        var targetDir = Paths.ReposRoot;
        
        Directory.CreateDirectory(targetDir);
        
        
        var repoFolderName = name.Split('/').Last();
        var fullRepoPath = Path.Combine(targetDir, repoFolderName);

        if (overwrite && Directory.Exists(fullRepoPath))
        {
            RemoveReadOnlyAttributes(fullRepoPath);
            Directory.Delete(fullRepoPath, true);
        }
        
        //var targetDir = "/app/repos";

        var gitProcess = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"clone https://github.com/{name}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = targetDir
        };
        using var process = new Process();
        process.StartInfo = gitProcess;
        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();
        process.Dispose();
        
        Console.WriteLine(output);
        if (!string.IsNullOrWhiteSpace(error))
            Console.Error.WriteLine(error);
    }
}