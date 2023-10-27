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
      settings.SourceUrl = "https://path.to.com/redirects.json"; // Required
      settings.Enabled = true;
      settings.CacheDurationInMinutes = 1;
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
