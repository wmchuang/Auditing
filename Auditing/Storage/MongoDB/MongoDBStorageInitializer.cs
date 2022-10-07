using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Auditing.Storage.MongoDB;

/// <summary>
/// Mongo库的初始化
/// </summary>
public class MongoDBStorageInitializer : IStorageInitializer
{
    private readonly IMongoClient _client;
    private readonly ILogger _logger;
    private readonly IOptions<MongoDBOptions> _options;

    public MongoDBStorageInitializer(
        ILogger<MongoDBStorageInitializer> logger,
        IMongoClient client,
        IOptions<MongoDBOptions> options)
    {
        _options = options;
        _logger = logger;
        _client = client;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var options = _options.Value;
        var database = _client.GetDatabase(options.DatabaseName);
        var names = (await database.ListCollectionNamesAsync(cancellationToken: cancellationToken)).ToList();

        //判断库中是否有集合。没有则创建
        if (names.All(n => n != options.Collection))
            await database.CreateCollectionAsync(options.Collection, cancellationToken: cancellationToken);

        await Task.WhenAll(
            TryCreateIndexesAsync<AuditLogInfo>(options.Collection));

        _logger.LogDebug("Ensuring all create database tables script are applied.");

        //创建集合索引
        async Task TryCreateIndexesAsync<T>(string collectionName)
        {
            var indexNames = new[] { "UserId", "TenantId" };
            var col = database.GetCollection<T>(collectionName);
            using (var cursor = await col.Indexes.ListAsync(cancellationToken))
            {
                var existingIndexes = await cursor.ToListAsync(cancellationToken);
                var existingIndexNames = existingIndexes.Select(o => o["name"].AsString).ToArray();
                indexNames = indexNames.Except(existingIndexNames).ToArray();
            }

            if (indexNames.Any() == false)
                return;

            var indexes = indexNames.Select(indexName =>
            {
                var indexOptions = new CreateIndexOptions
                {
                    Name = indexName,
                    Background = true
                };
                var indexBuilder = Builders<T>.IndexKeys;
                return new CreateIndexModel<T>(indexBuilder.Descending(indexName), indexOptions);
            }).ToArray();

            await col.Indexes.CreateManyAsync(indexes, cancellationToken);
        }
    }
}