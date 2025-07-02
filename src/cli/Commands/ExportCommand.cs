using System.ComponentModel;
using Spectre.Console.Cli;
using lib;

namespace cli.Commands;

public class ExportCommand : AsyncCommand<ExportCommand.GitExec>
{
    private readonly IRankingService _service;

    public ExportCommand(IRankingService service)
    {
        _service = service;
    }

    public class GitExec : CommandSettings
    {
/*
        [Description("Hash commit of export")]
        [CommandOption("-h|--hash")]
        public string? HashCommit { get; init; }
        */
        [Description("Export all commits")]
        [CommandOption("-a|--all")]
        [DefaultValue(false)]
        public bool All { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, GitExec settings)
    {
        if (settings.All)
        {
            await _service.FetchLatestChangesAsync();
            await _service.ExportHashCommit();
        }

        return 0;
    }
}