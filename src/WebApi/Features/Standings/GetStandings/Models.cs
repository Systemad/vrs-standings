using Features.Rankings.Models;

namespace Standings.GetStandings;

public class GetStandingsRequest
{
    public string Region { get; set; }
    public string Date { get; set; }
}

public class GetStandingsResponse
{
    public List<TeamStanding> Standings { get; set; } = new();
}