namespace IP.CC.RedirectMiddleware;

public class RedirectsSettings
{
	public int CacheDurationInMinutes { get; set; } = 5;
	public string SourceUrl { get; set; } = "";
    public bool Enabled { get; set; } = true;
}