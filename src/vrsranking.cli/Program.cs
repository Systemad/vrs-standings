using System.Diagnostics;
using GitReader;
using GitReader.Structures;

using StructuredRepository repository =
    await Repository.Factory.OpenStructureAsync(
        "C:\\Users\\yeahg\\source\\repos\\ProjectLudus");
/*
// Found current head
if (repository.Head is Branch head)
{
    Console.WriteLine($"Name: {head.Name}");

    // Get the commit that this HEAD points to:
    Commit commit = await head.GetHeadCommitAsync();

    Console.WriteLine($"Hash: {commit.Hash}");
    Console.WriteLine($"Author: {commit.Author}");
    Console.WriteLine($"Committer: {commit.Committer}");
    Console.WriteLine($"Subject: {commit.Subject}");
    Console.WriteLine($"Body: {commit.Body}");
}
*/
/*
if (await repository.GetCommitAsync(
        "3977a03def434131b50718d3d8ec5c46e831c760") is Commit commit)
{
    Console.WriteLine($"Hash: {commit.Hash}");
    Console.WriteLine($"Author: {commit.Author}");
    Console.WriteLine($"Committer: {commit.Committer}");
    Console.WriteLine($"Subject: {commit.Subject}");
    Console.WriteLine($"Body: {commit.Body}");
}
*/


static async ValueTask WhenAll(IEnumerable<ValueTask> tasks)
{
    foreach (var task in tasks.ToArray())
    {
        await task;
    }
}

static async Task ExportAsync(
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

    var directories = 0;
    var files = 0;

    static async ValueTask ExtractBlobAsync(
        TreeBlobEntry blob, string path)
    {
        var openBlobAsync = blob.OpenBlobAsync();

        // I don't know why basePath doesn't exist in randomly case.
        // The directory should always have been created by TreeDirectoryEntry before coming here.
        // There may be a potential problem in WSL environment...
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

    async ValueTask ExtractSubModule(TreeSubModuleEntry subModule, string basePath)
    {
        using var subModuleRepository = await subModule.OpenSubModuleAsync();

        var subModuleCommit = await subModuleRepository.GetCommitAsync(subModule);
        var subModuleRootTree = await subModuleCommit!.GetTreeRootAsync();

        await ExtractTreeAsync(subModuleRootTree.Children, basePath);
    }

    ValueTask ExtractTreeAsync(TreeEntry[] entries, string basePath) =>
        WhenAll(entries.Select(entry =>
        {
            var path = Path.Combine(basePath, entry.Name);
            switch (entry)
            {
                case TreeDirectoryEntry directory:
                    Interlocked.Increment(ref directories);
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
                    Interlocked.Increment(ref files);
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

    await ExtractTreeAsync(tree.Children, toPath);

    var extracted = sw.Elapsed;
    var e = extracted - gotTree;
    tw.WriteLine($"Extracted: {e}");

    tw.WriteLine();

    var dr = (double)directories / (directories + files);
    var d = TimeSpan.FromTicks((long)(e.Ticks * dr));
    tw.WriteLine($"Directories: {directories}, {d}");

    var fr = (double)files / (directories + files);
    var f = TimeSpan.FromTicks((long)(e.Ticks * fr));
    tw.WriteLine($"Files: {files}, {f}");
}