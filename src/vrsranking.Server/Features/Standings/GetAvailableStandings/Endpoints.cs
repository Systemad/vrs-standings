using FastEndpoints;
using vrsranking.lib;
using vrsranking.Server.Features.Standings;

namespace Standings.GetAvailableStandings;

public class GetAvailableStandingsResponse
{
    public string[] Dates { get; set; } = [];
}

public class GetAvailableStandingsRequest
{
    public string Region { get; set; }
}

public class Endpoints : Endpoint<GetAvailableStandingsRequest, GetAvailableStandingsResponse>
{
    public override void Configure()
    {
        Get("api/standings/available/{Region}");
        AllowAnonymous();        
    }

    public override async Task HandleAsync(GetAvailableStandingsRequest req, CancellationToken ct)
    {
        if (!Regions.Available.Contains(req.Region, StringComparer.InvariantCultureIgnoreCase))
        {
            ThrowError("Region doesnt exist!");
        }
        
        var file = $"{Paths.RankingsRepo}/live/2025";
        
        var dates = Directory
            .GetFiles(file, "standings_*.md", SearchOption.AllDirectories)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(name => name.Split('_'))
            .Where(parts => parts.Length == 5 && parts[1].Equals(req.Region, StringComparison.OrdinalIgnoreCase))
            .Select(parts => $"{parts[2]}_{parts[3]}_{parts[4]}")
            .Distinct()
            .OrderByDescending(d => d)
            .ToArray();

        await SendAsync(new GetAvailableStandingsResponse() { Dates = dates });
    }
}