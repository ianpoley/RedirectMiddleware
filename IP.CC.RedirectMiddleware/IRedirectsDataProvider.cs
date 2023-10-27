using IP.CC.RedirectMiddleware.Entities;

namespace IP.CC.RedirectMiddleware;

public interface IRedirectsDataProvider
{
	Task<IList<RedirectRule>> FetchDataAsync();
}