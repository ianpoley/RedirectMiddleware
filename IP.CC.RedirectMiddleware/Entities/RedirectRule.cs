using System.Text.Json.Serialization;

namespace IP.CC.RedirectMiddleware.Entities;

public class RedirectRule
{
	private string _redirectUrl = "";
	private string _targetUrl = "";

	[JsonPropertyName("redirectUrl")]
    public required string RedirectUrl
    {
	    get => _redirectUrl.EnsurePrefix('/').TrimEnd('/');
	    set => _redirectUrl = value;
    }

	[JsonPropertyName("targetUrl")]
    public required string TargetUrl
    {
	    get => _targetUrl.EnsurePrefix('/').TrimEnd('/');
	    set => _targetUrl = value;
    }

	[JsonPropertyName("redirectType")]
    public int RedirectType { get; set; }

    [JsonPropertyName("useRelative")]
    public bool UseRelative { get; set; }

    [JsonIgnore]
    public string DestinationUrl =>
        UseRelative
            ? RedirectUrl + TargetUrl
            : TargetUrl;
}
