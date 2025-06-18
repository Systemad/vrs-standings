using System.ComponentModel;
using Spectre.Console.Cli;

namespace vrsranking.cli;

public class GitCommand : Command<GitCommand.GitExec>
{
    public class GitExec : CommandSettings
    {

        
        [Description("Hash commit of export")]
        [CommandOption("-h|--hash")]
        public string? HashCommit { get; init; }
    }

    public override int Execute(CommandContext context, GitExec settings)
    {
        throw new NotImplementedException();
    }
}   