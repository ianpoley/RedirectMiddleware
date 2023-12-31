﻿using IP.CC.RedirectMiddleware;

namespace IP.CC.Web;

public class Startup
{
    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddMemoryCache();

        // Register the IRedirectsDataProvider service 
        services.AddSingleton<IRedirectsDataProvider>(sp =>
	        new SimpleRedirectsDataProvider(
		        sp.GetRequiredService<HttpClient>(),
		        sp.GetRequiredService<ILogger<SimpleRedirectsDataProvider>>(),
		        "https://raw.githubusercontent.com/ianpoley/RedirectMiddleware/main/redirects.json")
		);

		// Configure redirect service
		services.AddRedirectMiddleware(settings =>
        {
            // Configure redirect middleware options
	        settings.Enabled = true;
	        settings.CacheDurationInMinutes = 1;
		});

        services.AddRazorPages();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Configure redirect service middleware
        app.UseRedirectMiddleware();

        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });
    }
}