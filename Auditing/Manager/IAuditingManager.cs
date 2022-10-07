namespace Auditing.Manager;

public interface IAuditingManager
{
    /// <summary>
    /// 开始执行审计记录
    /// </summary>
    /// <returns></returns>
    Task BeginScope();

    /// <summary>
    /// 将审计试题持久化到存储库
    /// </summary>
    /// <returns></returns>
    Task SaveAsync();
}