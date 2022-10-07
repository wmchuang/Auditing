using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Auditing.Storage.MongoDB;

public class MongoDBAuditingStore : IAuditingStore
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDBAuditingStore> _logger;
    private readonly IOptions<MongoDBOptions> _options;

    public MongoDBAuditingStore(ILogger<MongoDBAuditingStore> logger, IOptions<MongoDBOptions> options, IMongoClient client)
    {
        _logger = logger;
        _options = options;
        _client = client;
        _database = _client.GetDatabase(_options.Value.DatabaseName);
    }

    /// <see cref="IAuditingStore.SaveAsync"/>
    public async Task SaveAsync(AuditLogInfo auditInfo)
    {
        try
        {
            var insertOptions = new InsertOneOptions { BypassDocumentValidation = false };
            var collection = _database.GetCollection<AuditLogInfo>(_options.Value.Collection);
            await collection.InsertOneAsync(auditInfo, insertOptions);
        }
        catch (Exception e)
        {
            _logger.LogError($"保存审计日志时出错{e.Message}");
        }
    }

    /// <see cref="IAuditingStore.DeleteExpiresAsync"/>
    public async Task<int> DeleteExpiresAsync(DateTime timeout, int batchCount = 1000, CancellationToken token = default)
    {
        var publishedCollection = _database.GetCollection<AuditLogInfo>(_options.Value.Collection);
        var ret = await publishedCollection.DeleteManyAsync(x => x.ExecutionTime < timeout, token);
        return (int)ret.DeletedCount;
    }
}