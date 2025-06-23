using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using vrsranking.cli;
using vrsranking.cli.Commands;
using vrsranking.lib;
using vrsranking.lib.GitLog;
using vrsranking.lib.GitRepo;

var services = new ServiceCollection();

services.AddSingleton<IGitRepoService, GitRepoService>();
services.AddSingleton<IGitLogService, GitLogService>();
services.AddSingleton<IRankingService, RankingService>();

var app = new CommandApp(new MyTypeRegistrar(services));

app.Configure(config =>
{
    config.AddCommand<InitCommand>("init")
        .WithExample("--init", "--name", "ValveSoftware/counter-strike_regional_standings");

    config.AddCommand<ExportCommand>("export");
});
await app.RunAsync(args);

/*

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
/*
using StructuredRepository repository =
    await Repository.Factory.OpenStructureAsync(
        "C:\\Users\\yeahg\\source\\repos\\ProjectLudus");
*/
// WriteLogAsync(Console.Out, path);
//await vrsranking.cli.Program.RunOp("C:\\Users\\yeahg\\source\\repos\\vrsranking");


//await ExportAsync(Console.Out, "C:\\Users\\yeahg\\source\\repos\\vrsranking", "05a39bb6de92a4e6296998c9e353b11e0a7ae007", "C:\\Users\\yeahg\\source\\repos\\vrsranking\\data\\05a39bb6de92a4e6296998c9e353b11e0a7ae007");