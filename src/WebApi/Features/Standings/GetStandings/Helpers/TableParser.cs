using Markdig;
using Markdig.Extensions.AutoLinks;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Features.Rankings.Models;

namespace GetStandings.Helpers;

public static class TableParser
{
    public static List<TeamStanding> GetTeams(string path)
    {
        string readText = File.ReadAllText(path);

        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseAutoLinks(new AutoLinkOptions { OpenInNewWindow = true })
            .Build();

        var document = Markdown.Parse(readText, pipeline);


        var rows = document.Descendants<Table>().SelectMany(t => t.Descendants<TableRow>()).Skip(1);
        var rowsvalue = new List<TeamStanding>();

        foreach (var row in rows)
        {
            var cells = row.Descendants<TableCell>().ToArray();


            var standing = GetCellText(cells[0]);
            var points = GetCellText(cells[1]);
            var teamName = GetCellText(cells[2]);
            var roster = GetCellText(cells[3]);
            var details = ParseDetails(cells[4]);

            var players = roster.Trim().Replace(" ", "").Split(",");

            rowsvalue.Add(new TeamStanding
            {
                Standings = standing,
                Points = points,
                TeamName = teamName,
                Roster = players,
                Details = details
            });
        }

        return rowsvalue;
    }

    private static string GetCellText(TableCell cell)
    {
        var parts = new List<string>();

        foreach (var inline in cell.Descendants<Inline>())
        {
            switch (inline)
            {
                case LiteralInline literal:
                    parts.Add(literal.Content.ToString());
                    break;
                case LinkInline link:
                    // Combine label and destination
                    parts.Add($"[{link.FirstChild?.ToString()}]({link.Url})");
                    break;
            }
        }

        return string.Join("", parts);
    }

    private static Details ParseDetails(TableCell cell)
    {
        var link = cell.Descendants<LinkInline>().FirstOrDefault();
        if (link == null || string.IsNullOrWhiteSpace(link.Url))
            return null;

        // Extract just the part after "details/"
        var url = link.Url;
        var trimmed = url.StartsWith("details/") ? url.Substring("details/".Length) : url;

        // Now split into date and name
        var parts = trimmed.Split('/', 2);
        if (parts.Length != 2)
            return null;

        return new Details
        {
            Key = parts[0], // e.g., "2025_06_02"
            Filename = parts[1] // e.g., "0001--vitality--apex-flamez-mezii-ropz-zywoo.md"
        };
    }
}