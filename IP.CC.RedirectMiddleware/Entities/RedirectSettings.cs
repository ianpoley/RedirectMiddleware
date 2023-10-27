namespace IP.CC.RedirectMiddleware.Entities;

public class RedirectsSettings
{
    public int CacheDurationInMinutes { get; set; } = 5;
    public string RedirectsSourceUrl { get; set; } = "";
    public bool Enabled { get; set; } = true;
}