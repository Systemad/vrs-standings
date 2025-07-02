using FastEndpoints;
using GetStandings.Helpers;
using lib;

namespace Standings.GetStandings;


public class Endpoint : Endpoint<GetStandingsRequest, GetStandingsResponse>
{
    public override void Configure()
    {
        Get("api/standings/{Region}/{Date}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetStandingsRequest req, CancellationToken ct)
    {
        var file = $"{Paths.RankingsRepo}/live/2025/standings_{req.Region}_{req.Date}.md";
        
        if (!File.Exists(file))
        {
            ThrowError("Standings doesnt exist!");
        }

        var standings = TableParser.GetTeams(file);
        await SendAsync(new GetStandingsResponse()
        {
            Standings = standings
        });
    }
}