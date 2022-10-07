namespace Auditing.Storage.Mysql;

public class MySqlOptions
{
    /// <summary>
    /// Gets or sets the database's connection string that will be used to store database entities.
    /// </summary>
    public string ConnectionString { get; set; }
    
    /// <summary>
    /// 表名称
    /// </summary>
    public string TableName { get; set; } = "AuditLogInfo";
}