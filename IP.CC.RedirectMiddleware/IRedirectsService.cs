﻿using IP.CC.RedirectMiddleware.Entities;

namespace IP.CC.RedirectMiddleware;

public interface IRedirectsService
{
    RedirectsSettings Settings { get; set; }
    Task<IList<RedirectRule>> GetDataAsync();
}
