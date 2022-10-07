using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Auditing.Storage.Mysql;

public class MySqlAuditingStore : IAuditingStore
{
    private readonly ILogger<MySqlAuditingStore> _logger;
    private readonly IOptions<MySqlOptions> _options;

    public MySqlAuditingStore(ILogger<MySqlAuditingStore> logger, IOptions<MySqlOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    /// <see cref="IAuditingStore.SaveAsync"/>
    public async Task SaveAsync(AuditLogInfo auditInfo)
    {
        try
        {
            var sql = $"INSERT INTO `{_options.Value.TableName}` (Id, UserId, UserName, TenantId, TenantName, ExecutionTime, ExecutionDuration, ClientIpAddress, BrowserInfo, HttpMethod, HttpStatusCode, Url, Body)" +
                      $"VALUES(@Id, @UserId, @UserName, @TenantId,@TenantName, @ExecutionTime, @ExecutionDuration,@ClientIpAddress,@BrowserInfo,@HttpMethod,@HttpStatusCode,@Url,@Body );";

            object[] sqlParams =
            {
                new MySqlParameter("@Id", auditInfo.Id),
                new MySqlParameter("@UserId", auditInfo.UserId),
                new MySqlParameter("@UserName", auditInfo.UserName),
                new MySqlParameter("@TenantId", auditInfo.TenantId),
                new MySqlParameter("@TenantName", auditInfo.TenantName),
                new MySqlParameter("@ExecutionTime", auditInfo.ExecutionTime),
                new MySqlParameter("@ExecutionDuration", auditInfo.ExecutionDuration),
                new MySqlParameter("@ClientIpAddress", auditInfo.ClientIpAddress),
                new MySqlParameter("@BrowserInfo", auditInfo.BrowserInfo),
                new MySqlParameter("@HttpMethod", auditInfo.HttpMethod),
                new MySqlParameter("@HttpStatusCode", auditInfo.HttpStatusCode),
                new MySqlParameter("@Url", auditInfo.Url),
                new MySqlParameter("@Body", auditInfo.Body),
            };
            await using var connection = new MySqlConnection(_options.Value.ConnectionString);
            connection.ExecuteNonQuery(sql, sqlParams: sqlParams);
        }
        catch (Exception e)
        {
            _logger.LogError($"保存审计日志时出错{e.Message}");
        }
    }

    /// <see cref="IAuditingStore.DeleteExpiresAsync"/>
    public async Task<int> DeleteExpiresAsync(DateTime timeout, int batchCount = 1000, CancellationToken token = default)
    {
        await using var connection = new MySqlConnection(_options.Value.ConnectionString);
        return connection.ExecuteNonQuery(
            $@"DELETE FROM `{_options.Value.TableName}` WHERE ExecutionTime < @timeout limit @batchCount;", null,
            new MySqlParameter("@timeout", timeout), new MySqlParameter("@batchCount", batchCount));
    }
}