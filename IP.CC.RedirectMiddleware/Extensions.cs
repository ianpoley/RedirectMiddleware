using IP.CC.RedirectMiddleware.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IP.CC.RedirectMiddleware;

public static class Extensions
{
	public static IApplicationBuilder UseRedirectMiddleware(this IApplicationBuilder app)
	{
		return app.UseMiddleware<RedirectMiddleware>();
	}

	public static IServiceCollection AddRedirectMiddleware(
		this IServiceCollection services,
		Action<RedirectsSettings> options)
	{
		// Configure IOptions<RedirectsSettings>
		services.Configure<RedirectsSettings>(options);

		// Add IRedirectsService to DI
		services.AddTransient<IRedirectsService, RedirectsService>();

		// Register background service to refresh cache without HTTP request(s)
		services.AddHostedService<RedirectsBackgroundService>();

		return services;
	}
}
