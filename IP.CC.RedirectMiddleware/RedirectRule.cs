using System.Text.Json.Serialization;

namespace IP.CC.RedirectMiddleware;

public class RedirectRule
{
	[JsonPropertyName("redirectUrl")]
	public required string RedirectUrl { get; set; }

	[JsonPropertyName("targetUrl")]
	public required string TargetUrl { get; set; }

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
