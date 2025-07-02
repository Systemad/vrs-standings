using FastEndpoints;
using lib;

namespace Standings.GetDetail;

public class GetDetailRequest
{
    public string Filename { get; set; }
    public string Date { get; set; }
}

public class GetDetailResponse
{
    public string MarkdownString { get; set; }
}

public class Endpoint : Endpoint<GetDetailRequest, GetDetailResponse>
{
    public override void Configure()
    {
        Get("api/markdown/{Date}/{Filename}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetDetailRequest req, CancellationToken ct)
    {
        var markdown = Path.Combine(Paths.Rankings.Y2025Details, req.Date, req.Filename);

        if (!File.Exists(markdown))
        {
            ThrowError("Markdown doesn't exist!");
        }

        var fileString = await File.ReadAllTextAsync(markdown, ct);

        await SendAsync(new GetDetailResponse()
        {
            MarkdownString = fileString
        });
    }
}