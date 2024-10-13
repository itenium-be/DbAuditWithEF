using Microsoft.AspNetCore.Http;

namespace DbAuditWithEF.ByEF;

public interface IUserProvider
{
    string UserName { get; }
}

public class HttpUserProvider(IHttpContextAccessor accessor) : IUserProvider
{
    public string UserName => accessor.HttpContext?.User?.Identity?.Name ?? "???";
}

public class CronJobUserProvider : IUserProvider
{
    public string UserName => "CronJob";
}
