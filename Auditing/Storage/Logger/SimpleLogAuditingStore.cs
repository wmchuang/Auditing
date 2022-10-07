using Microsoft.Extensions.Logging;

namespace Auditing.Storage.Logger;

public class SimpleLogAuditingStore : IAuditingStore
{
    private readonly ILogger<SimpleLogAuditingStore> _logger;

    public SimpleLogAuditingStore(ILogger<SimpleLogAuditingStore> logger)
    {
        _logger = logger;
    }

    public Task SaveAsync(AuditLogInfo auditInfo)
    {
        _logger.LogInformation(auditInfo.ToString());
        return Task.FromResult(0);
    }

    public Task<int> DeleteExpiresAsync(DateTime timeout, int batchCount = 1000, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}