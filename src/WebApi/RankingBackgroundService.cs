namespace WebApi;

public class RankingBackgroundService : BackgroundService
{
    private readonly ILogger<RankingBackgroundService> _logger;

    public RankingBackgroundService(ILogger<RankingBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RankingBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Running git pull at: {time}", DateTimeOffset.Now);

                // Call your git pull logic here, e.g., run shell command or your method
                await RunGitPullAsync();

                _logger.LogInformation("Git pull completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while running git pull.");
            }

            // Wait 30 minutes or until cancellation
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }

        _logger.LogInformation("RankingBackgroundService is stopping.");
    }

    private Task RunGitPullAsync()
    {
        // Implement your git pull logic here
        // Example: run 'git pull' in your repo directory via Process.Start, or call a service method

        return Task.CompletedTask; // placeholder
    }
}