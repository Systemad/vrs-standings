// See https://aka.ms/new-console-template for more information

using Markdig;
using Markdig.Extensions.AutoLinks;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using lib;


string path = @"C:\Users\yeahg\source\repos\vrsranking\src\ConsoleApp\standings_global_2025_06_02.md";
string readText = File.ReadAllText(path);

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseAutoLinks(new AutoLinkOptions { OpenInNewWindow = true })
    .Build();

var document = Markdown.Parse(readText, pipeline);

// Get the first table (or loop all if needed)
var tables = document.Descendants<Table>();

var result = Markdown.Parse(readText, pipeline);

var rows = result.Descendants<Table>().SelectMany(t => t.Descendants<TableRow>()).Skip(1);

var rowsvalue = new List<TableEntry>();

foreach (var row in rows)
{
    var cells = row.Descendants<TableCell>().ToArray();

    string GetCellText(TableCell cell)
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

    Details ParseDetails(TableCell cell)
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

    var standing = GetCellText(cells[0]);
    var points = GetCellText(cells[1]);
    var teamName = GetCellText(cells[2]);
    var roster = GetCellText(cells[3]);
    var details = ParseDetails(cells[4]);

    var players = roster.Trim().Split(",");

    rowsvalue.Add(new TableEntry
    {
        Standings = standing,
        Points = points,
        TeamName = teamName,
        Roster = players,
        Details = details
    });
}

var file = $"{Paths.RankingsRepo}/live/2025";

var dates = Directory
    .GetFiles(file, "standings_*.md", SearchOption.AllDirectories)
    .Select(Path.GetFileNameWithoutExtension)
    .Select(name => name.Split('_'))
    .Where(parts => parts.Length == 5 && parts[1].Equals("asia", StringComparison.OrdinalIgnoreCase))
    .Select(parts => $"{parts[2]}_{parts[3]}_{parts[4]}")
    .Distinct()
    .OrderByDescending(d => d)
    .ToList();

foreach (var date in dates)
{
    Console.WriteLine(date);
}
Console.WriteLine(dates.Count());
//foreach (var item in rowsvalue)
//{
//    Console.WriteLine($"{item.TeamName}");
//}

/*
string path = @"C:\Users\yeahg\source\repos\vrsranking\src\ConsoleApp\standings_global_2025_06_02.md";

// Open the file to read from.
string readText = File.ReadAllText(path);

var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions()
    .UseAutoLinks(new AutoLinkOptions() { OpenInNewWindow = true }).Build();

var result = Markdown.Parse(readText, pipeline);

var tabl2 = result.Descendants<Table>().SelectMany(t => t.Descendants<TableRow>()).Skip(1);
foreach (var tblcell in tabl2)
{
    var cells = tblcell.Descendants<TableCell>().ToArray();
    Console.WriteLine(cells[0].Descendants<LiteralInline>().ToArray()[0]);
    Console.WriteLine(cells[1].Descendants<LiteralInline>().ToArray()[0]);
    Console.WriteLine(cells[2].Descendants<LiteralInline>().ToArray()[0]);
    Console.WriteLine(cells[3].Descendants<LiteralInline>().ToArray()[0]);
    Console.WriteLine(cells[4].Descendants<LiteralInline>().ToArray()[0]);

}
*/
/*
foreach (var node in table.First())
{

    var cells = node.Descendants<TableCell>().ToArray();
    Console.WriteLine(cells[1].ToString());
    //Console.WriteLine(node.Parser.);
}
*/

public class Details
{
    public string Key { get; set; }
    public string Filename { get; set; }
}


public class TableEntry
{
    public string Standings { get; set; }
    public string Points { get; set; }
    public string TeamName { get; set; }
    public string[] Roster { get; set; }
    public Details Details { get; set; }
}