using IP.CC.RedirectMiddleware;
using IP.CC.RedirectMiddleware.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IP.CC.Web.Pages;

public class IndexModel : PageModel
{
	private readonly IRedirectsService _redirectsService;

	public IndexModel(IRedirectsService redirectsService)
	{
		_redirectsService = redirectsService;

		IsEnabled = _redirectsService.Settings.Enabled;
		CacheDurationInMinutes = _redirectsService.Settings.CacheDurationInMinutes;
	}

	public IList<RedirectRule> RedirectRules { get; set; }

	public bool IsEnabled { get; set; }

	public int CacheDurationInMinutes { get; set; }

	public async Task OnGet()
	{
		RedirectRules = await _redirectsService.GetDataAsync();
	}
}