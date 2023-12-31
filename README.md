# Redirect Middleware

## How to configure

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient();
    services.AddMemoryCache();

    // Register the IRedirectsDataProvider service 
    services.AddScoped<IRedirectsDataProvider>(sp =>
        new RedirectsDataProvider(
            sp.GetRequiredService<HttpClient>(),
            sp.GetRequiredService<ILogger<RedirectsDataProvider>>(),
            "https://raw.githubusercontent.com/ianpoley/RedirectMiddleware/main/redirects.json")
    );

    // Configure redirect service
    services.AddRedirectMiddleware(settings =>
    {
        // Configure redirect middleware options
        settings.Enabled = true;
        settings.CacheDurationInMinutes = 1;
    });

    ... 
}
```

```cs
public void Configure(
    IApplicationBuilder app,
    IWebHostEnvironment env)
{
    ...    

    // Configure redirect service middleware
    app.UseRedirectMiddleware();

    ...
}
```
