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
    private readonly ILogger<RedirectsService> _logger;
    private readonly HttpClient _httpClient;

    public RedirectsService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<RedirectsService> logger,
        IOptions<RedirectsSettings> redirectsSettings)
    {
        _cache = cache;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
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
            var results = await GetDataFromRestApi();

            _logger.LogInformation($"Cache updated - {results.Count} entries");

            return results;
        }) ?? Enumerable.Empty<RedirectRule>().ToList();
    }

    private async Task<IList<RedirectRule>> GetDataFromRestApi()
    {
        // Throw an exception if the source url is blank
	    if (string.IsNullOrEmpty(Settings.SourceUrl))
	    {
		    throw new ArgumentException("The SourceUrl cannot be null or empty.", nameof(Settings.SourceUrl));
	    }

		try
        {
            // Send HTTP request
            var response = await _httpClient.GetAsync(Settings.SourceUrl);
            response.EnsureSuccessStatusCode();

            // Deserialize JSON
            var json = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<IList<RedirectRule>>(json);

            _logger.LogInformation("Redirects successfully fetched from API");

            return results == null 
	            ? Enumerable.Empty<RedirectRule>().ToList() 
	            : results.DistinctBy(i => i.RedirectUrl).ToList();
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "An error occurred while sending the HTTP request.");
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "An error occurred while deserializing the JSON response.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred.");
        }

        return Enumerable.Empty<RedirectRule>().ToList();
    }
}