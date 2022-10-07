using Auditing.Manager;
using Auditing.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Auditing;

/// <summary>
/// 审计日志中间件
/// </summary>
public class AuditingMiddleware : IMiddleware
{
    private readonly IAuditingManager _auditingManager;
    private readonly IOptions<AuditingOptions> _options;

    public AuditingMiddleware(IAuditingManager auditingManager, IOptions<AuditingOptions> options)
    {
        _auditingManager = auditingManager;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_options.Value.IsEnabled)
        {
            await next(context);
            return;
        }

        await _auditingManager.BeginScope();
        try
        {
            await next(context);
        }
        finally
        {
            await _auditingManager.SaveAsync();
        }
    }
}