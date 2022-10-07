using Auditing.Options;

namespace Auditing.Storage.Mysql;

public static class AuditingOptionsExtensions
{
    public static AuditingOptions UseMySql(this AuditingOptions options, string connectionString)
    {
        return options.UseMySql(opt => { opt.ConnectionString = connectionString; });
    }

    public static AuditingOptions UseMySql(this AuditingOptions options, Action<MySqlOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        options.RegisterExtension(new MySqlAuditingOptionsExtension(configure));
        return options;
    }
}