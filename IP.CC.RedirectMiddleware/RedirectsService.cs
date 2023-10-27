using IP.CC.RedirectMiddleware.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IP.CC.RedirectMiddleware;

public class RedirectsService : IRedirectsService
{
	private const string CacheKey = "RedirectRules";

	private readonly IMemoryCache _cache;
	private readonly IRedirectsDataProvider _redirectsDataProvider;
	private readonly ILogger<RedirectsService> _logger;

	public RedirectsService(
		IMemoryCache cache,
		IRedirectsDataProvider redirectsDataProvider,
		ILogger<RedirectsService> logger,
		IOptions<RedirectsSettings> redirectsSettings)
	{
		_cache = cache;
		_logger = logger;
		_redirectsDataProvider = redirectsDataProvider;

		Settings = redirectsSettings.Value;

		// Build initial cache
		var redirectResults = GetDataAsync().Result;
	}

	public RedirectsSettings Settings { get; set; }

	public async Task<IList<RedirectRule>> GetDataAsync()
	{
		return await _cache.GetOrCreateAsync(CacheKey, async entry =>
		{
			// Set cache options
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Settings.CacheDurationInMinutes);

			// Query the data source
			var results = await _redirectsDataProvider.FetchJsonDataAsync();

			_logger.LogInformation($"Cache updated - {results.Count} entries");

			return results;
		}) ?? Enumerable.Empty<RedirectRule>().ToList();
	}
}