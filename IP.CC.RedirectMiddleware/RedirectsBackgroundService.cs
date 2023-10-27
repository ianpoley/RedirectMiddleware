using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IP.CC.RedirectMiddleware;

public class RedirectsBackgroundService : BackgroundService
{
	private readonly SemaphoreSlim _lock = new(1, 1);
	private readonly ILogger<RedirectsBackgroundService> _logger;
	private readonly IRedirectsService _redirectsService;

	public RedirectsBackgroundService(
		ILogger<RedirectsBackgroundService> logger,
		IRedirectsService redirectsService)
	{
		_logger = logger;
		_redirectsService = redirectsService;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (_redirectsService.Settings.Enabled)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var lockTaken = false;

				try
				{
					await _lock.WaitAsync(stoppingToken);

					lockTaken = true;

					// Fetch redirects & force cache to update if expired
					var redirectResults = await _redirectsService.GetDataAsync();

					//_logger.LogInformation($"Completed - {redirectResults?.Count} entries");
				}
				finally
				{
					if (lockTaken)
					{
						_lock.Release();
					}
				}

				// Recursively call task 
				await Task.Delay(TimeSpan.FromMinutes(_redirectsService.Settings.CacheDurationInMinutes), stoppingToken);
			}
		}
	}
}