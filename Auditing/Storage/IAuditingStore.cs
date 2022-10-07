namespace Auditing.Storage;

public interface IAuditingStore
{
    /// <summary>
    /// 保存审计记录
    /// </summary>
    /// <param name="auditInfo"></param>
    /// <returns></returns>
    Task SaveAsync(AuditLogInfo auditInfo);

    /// <summary>
    /// 删除过期记录
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="batchCount"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<int> DeleteExpiresAsync(DateTime timeout, int batchCount = 1000, CancellationToken token = default);
}