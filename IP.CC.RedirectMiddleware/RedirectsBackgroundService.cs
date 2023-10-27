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
		// Check if the redirects service is enabled
		if (_redirectsService.Settings.Enabled)
		{
			// Keep running until a cancellation is requested
			while (!stoppingToken.IsCancellationRequested)
			{
				// Variable to track if the lock was successfully acquired
				var lockTaken = false;

				try
				{
					// Acquire a lock to ensure thread-safety
					await _lock.WaitAsync(stoppingToken);

					// Indicate that the lock has been taken
					lockTaken = true;

					// Fetch redirects and force cache to update if expired
					var redirectResults = await _redirectsService.GetDataAsync();

					// Uncomment the following line to log the count of fetched entries
					//_logger.LogInformation($"Completed - {redirectResults?.Count} entries");
				}
				finally
				{
					// If the lock was taken, release it to allow other threads to proceed
					if (lockTaken)
					{
						_lock.Release();
					}
				}

				// Delay the task for the specified cache duration before the next iteration,
				// allowing for periodic updates of the redirect data
				await Task.Delay(TimeSpan.FromMinutes(_redirectsService.Settings.CacheDurationInMinutes), stoppingToken);
			}
		}
	}
}