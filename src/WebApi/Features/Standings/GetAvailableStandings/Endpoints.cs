using FastEndpoints;
using lib;
using WebApi.Features.Standings;

namespace Standings.GetAvailableStandings;

public class AvailableRegionsWithDates
{
    public string Region { get; set; }
    public List<string> Dates { get; set; } = new();
}

public class GetAvailableStandingsResponse
{
    public List<AvailableRegionsWithDates> RegionWithDates { get; set; } = new();
}

public class Endpoints : EndpointWithoutRequest<GetAvailableStandingsResponse>
{
    public override void Configure()
    {
        Get("api/standings/available");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        /*
        if (!Regions.Available.Contains(req.Region, StringComparer.InvariantCultureIgnoreCase))
        {
            ThrowError("Could not fetch regions with dates");
        }
        */
        var file = $"{Paths.RankingsRepo}/live/2025";

        var regionDates = Regions.Available.Select(region =>
        {
            var dates = Directory
                .GetFiles(file, "standings_*.md", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
                .Select(name => name.Split('_'))
                .Where(parts => parts.Length == 5 && parts[1].Equals(region, StringComparison.OrdinalIgnoreCase))
                .Select(parts => $"{parts[2]}_{parts[3]}_{parts[4]}")
                .Distinct()
                .OrderByDescending(d => d).ToList();
            return new AvailableRegionsWithDates()
            {
                Region = region,
                Dates = dates
            };
        }).ToList();


        await SendAsync(new GetAvailableStandingsResponse()
        {
            RegionWithDates = regionDates
        });
    }
}