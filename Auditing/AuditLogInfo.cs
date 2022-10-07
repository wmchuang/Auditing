using System.Text;
using Auditing.Storage.Mysql;

namespace Auditing;

/// <summary>
/// 审计日志实体
/// </summary>
[Serializable]
public class AuditLogInfo
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; } = SnowflakeId.Default().NextId().ToString();

    /// <summary>
    /// 当前用户的Id,用户未登录为 null
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 当前用户的用户名,如果用户已经登录
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 当前租户的Id,对于多租户应用
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// 当前租户的名称,对于多租户应用.
    /// </summary>
    public string? TenantName { get; set; }

    /// <summary>
    /// 审计日志对象创建的时间.
    /// </summary>
    public DateTime ExecutionTime { get; set; }

    /// <summary>
    /// 请求的总执行时间,以毫秒为单位. 可以用来观察应用程序的性能.
    /// </summary>
    public int ExecutionDuration { get; set; }

    /// <summary>
    /// 客户端/用户设备的IP地址.
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// 当前用户的浏览器名称/版本信息,如果有的话.
    /// </summary>
    public string? BrowserInfo { get; set; }

    /// <summary>
    /// 当前HTTP请求的方法(GET,POST,PUT,DELETE ...等).
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// HTTP响应状态码.
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// 请求的URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    public string? Body { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"AUDIT LOG: [{HttpStatusCode?.ToString() ?? "---"}: {(HttpMethod ?? "-------").PadRight(7)}] {Url}");
        sb.AppendLine($"- UserName - UserId      : {UserName} - {UserId}");
        sb.AppendLine($"- ClientIpAddress        : {ClientIpAddress}");
        sb.AppendLine($"- ExecutionDuration      : {ExecutionDuration}");
        sb.AppendLine($"- BrowserInfo      : {BrowserInfo}");
        sb.AppendLine($"- Body      : {Body}");

        return sb.ToString();
    }
}