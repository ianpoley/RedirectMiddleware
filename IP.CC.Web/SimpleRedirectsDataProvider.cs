using System.Text.Json;
using IP.CC.RedirectMiddleware;
using IP.CC.RedirectMiddleware.Entities;

namespace IP.CC.Web;

public class SimpleRedirectsDataProvider : IRedirectsDataProvider
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<SimpleRedirectsDataProvider> _logger;
	private readonly string _url;

	public SimpleRedirectsDataProvider(
		HttpClient httpClient,
		ILogger<SimpleRedirectsDataProvider> logger,
		string url)
	{
		_httpClient = httpClient;
		_url = url;
		_logger = logger;
	}

	public async Task<IList<RedirectRule>> FetchDataAsync()
	{
		try
		{
			// Send HTTP request
			var response = await _httpClient.GetAsync(_url);
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