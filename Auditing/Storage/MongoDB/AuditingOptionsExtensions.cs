using Auditing.Options;

namespace Auditing.Storage.MongoDB;

public static class AuditingOptionsExtensions
{
    public static AuditingOptions UseMongoDB(this AuditingOptions options, string connectionString)
    {
        return options.UseMongoDB(x => { x.DatabaseConnection = connectionString; });
    }

    public static AuditingOptions UseMongoDB(this AuditingOptions options, Action<MongoDBOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        options.RegisterExtension(new MongoDBAuditingOptionsExtension(configure));

        return options;
    }
}