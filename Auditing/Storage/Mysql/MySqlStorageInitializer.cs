using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Auditing.Storage.Mysql;

public class MySqlStorageInitializer : IStorageInitializer
{
    private readonly IOptions<MySqlOptions> _options;
    private readonly ILogger _logger;

    public MySqlStorageInitializer(
        ILogger<MySqlStorageInitializer> logger,
        IOptions<MySqlOptions> options)
    {
        _options = options;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        
        var sql = CreateDbTablesScript();
        await using var connection = new MySqlConnection(_options.Value.ConnectionString);
        connection.ExecuteNonQuery(sql);
        
        _logger.LogDebug("Ensuring all create database tables script are applied.");
    }

    /// <summary>
    /// 获取创建表sql
    /// </summary>
    /// <returns></returns>
    protected virtual string CreateDbTablesScript()
    {
        var sql = $@"
CREATE TABLE IF NOT EXISTS `{_options.Value.TableName}` (
  `Id` varchar(36) NOT NULL COMMENT '主键',
  `UserId` varchar(36) ,
  `UserName` varchar(50),
  `TenantId` varchar(36) ,
  `TenantName` varchar(50),
  `ExecutionTime` datetime NOT NULL,
  `ExecutionDuration` int(11) NOT NULL,
  `ClientIpAddress`  varchar(36) NOT NULL ,
  `BrowserInfo`  varchar(500) NOT NULL ,
  `HttpMethod`  varchar(20) NOT NULL ,
  `HttpStatusCode`  int(11),
  `Url`  longtext,
  `Body`  longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
";
        return sql;
    }
}