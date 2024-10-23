using Microsoft.AspNetCore.Http;

namespace DbAuditWithEF.Utils;

/// <summary>
/// An abstraction to inject the "current user" into the Db Audit fields
/// </summary>
public interface IUserProvider
{
    string UserName { get; }
}

/// <summary>
/// How this could typically be implemented in an ASP.NET context
/// </summary>
public class HttpUserProvider(IHttpContextAccessor accessor) : IUserProvider
{
    public string UserName => accessor.HttpContext?.User?.Identity?.Name ?? "???";
}

/// <summary>
/// Simple "UserName" provider with a hardcoded value
/// </summary>
public class CronJobUserProvider : IUserProvider
{
    public string UserName => "CronJob";
}
