using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Auditing.Storage;
using Auditing.Storage.Mysql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Auditing.Manager;

/// <summary>
/// 审计日志管理
/// </summary>
public class AuditingManager : IAuditingManager
{
    private readonly IAuditingStore _auditingStore;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private AuditLogInfo _auditLogInfo;
    private Stopwatch _stopwatch;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auditingStore"></param>
    /// <param name="httpContextAccessor"></param>
    public AuditingManager(IAuditingStore auditingStore, IHttpContextAccessor httpContextAccessor)
    {
        _auditingStore = auditingStore;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task BeginScope()
    {
        _auditLogInfo = new AuditLogInfo
        {
            ExecutionTime = DateTime.Now
        };

        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null)
        {
            _auditLogInfo.HttpMethod = httpContext.Request?.Method ?? string.Empty;
            _auditLogInfo.Url = httpContext.Request?.GetDisplayUrl() ?? string.Empty;
            _auditLogInfo.BrowserInfo = httpContext.Request?.Headers["User-Agent"] ?? string.Empty;
            _auditLogInfo.ClientIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            _auditLogInfo.HttpStatusCode = httpContext.Response.StatusCode;
            _auditLogInfo.Body = await ReadRequest(httpContext);
            var currentUser = httpContext.User;
            if (currentUser.HasClaim(x => x.Type == ClaimTypes.NameIdentifier))
            {
                _auditLogInfo.UserId = currentUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                _auditLogInfo.UserName = currentUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
                _auditLogInfo.TenantId = currentUser.Claims.FirstOrDefault(x => x.Type == "TenantId")?.Value ?? string.Empty;
                _auditLogInfo.TenantName = currentUser.Claims.FirstOrDefault(x => x.Type == "TenantName")?.Value ?? string.Empty;
            }
        }

        _stopwatch = Stopwatch.StartNew();
    }

    public virtual async Task SaveAsync()
    {
        BeforeSave();
        await _auditingStore.SaveAsync(_auditLogInfo);
    }

    private void BeforeSave()
    {
        _stopwatch.Stop();
        _auditLogInfo.ExecutionDuration = Convert.ToInt32(_stopwatch.Elapsed.TotalMilliseconds);
    }
    
    /// <summary>
    /// 读取请求数据
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task<string> ReadRequest(HttpContext context)
    {
        var request = context.Request;
      
        if (!(request.ContentLength > 0)) return string.Empty;
        request.Body.Position = 0;
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer).Replace("\r\n", "\n").Replace("\n","");
        request.Body.Position = 0;
        return bodyAsText;
    }
}