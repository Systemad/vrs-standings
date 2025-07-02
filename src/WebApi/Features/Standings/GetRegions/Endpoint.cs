using FastEndpoints;
using WebApi.Features.Standings;

namespace Standings.GetRegions;

public class GetRegionsResponse
{
    public string[] Regions { get; set; } = [];
}

public class Endpoint : EndpointWithoutRequest<GetRegionsResponse>
{
    public override void Configure()
    {
        Get("api/standings/regions");
        AllowAnonymous();        
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync(new GetRegionsResponse() { Regions = Regions.Available });
    }
}