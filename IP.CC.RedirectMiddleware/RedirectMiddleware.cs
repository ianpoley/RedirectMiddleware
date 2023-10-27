using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IP.CC.RedirectMiddleware;

public class RedirectMiddleware
{
	private readonly ILogger<RedirectMiddleware> _logger;
	private readonly RequestDelegate _next;
	private readonly IRedirectsService _redirectsService;

	public RedirectMiddleware(
		RequestDelegate next,
		IRedirectsService redirectsService,
		ILogger<RedirectMiddleware> logger)
	{
		_next = next;
		_redirectsService = redirectsService;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		if (_redirectsService.Settings.Enabled)
		{
			// Get redirect rules from service
			var redirectRules = await _redirectsService.GetDataAsync();

			// Find a matching redirect rule based on the requested url
			var ruleMatch = redirectRules
				.FirstOrDefault(i =>
					i.RedirectUrl.Equals(context.Request.Path, StringComparison.OrdinalIgnoreCase));

			if (ruleMatch != null)
			{
				// Determine where to redirect the request
				var targetUrl = ruleMatch.DestinationUrl;

				// Pull querystring through to new URL
				if (!string.IsNullOrEmpty(context.Request.QueryString.Value))
					targetUrl += context.Request.QueryString.Value;

				// Log redirect
				_logger.LogInformation($"Redirected {context.Request.Path}{context.Request.QueryString.Value} to {targetUrl}");

				// Execute redirect
				context.Response.Redirect(targetUrl, ruleMatch.RedirectType == 301);

				return;
			}
		}

		await _next(context);
	}
}