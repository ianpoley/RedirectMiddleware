# Redirect Middleware

## How to configure

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient();
    services.AddMemoryCache();

    // Configure redirect service
    services.AddRedirectMiddleware(settings =>
    {
      // Set options
      settings.Enabled = true;
      settings.CacheDurationInMinutes = 1;
      settings.SourceUrl = "https://raw.githubusercontent.com/ianpoley/RedirectMiddleware/main/redirects.json";
    });

    ... 
}
```

```
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
