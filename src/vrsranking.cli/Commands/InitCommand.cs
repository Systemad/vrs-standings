using System.ComponentModel;
using Spectre.Console.Cli;
using vrsranking.lib.GitRepo;

namespace vrsranking.cli.Commands;

public class InitCommand : AsyncCommand<InitCommand.Settings>
{
    private readonly IGitRepoService _repoService;

    public InitCommand(IGitRepoService repoService)
    {
        _repoService = repoService;
    }

    public class Settings : CommandSettings
    {
        [Description("Name of the repository to initialize")]
        [CommandOption("-n|--name <NAME>")]
        public string Name { get; init; }
        
        [Description("Overwrite if the repository already exists")]
        [CommandOption("-o|--overwrite")]
        [DefaultValue(false)]
        public bool Overwrite { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await _repoService.InitRepoAsync(settings.Name, settings.Overwrite);
        return 0;
    }
}