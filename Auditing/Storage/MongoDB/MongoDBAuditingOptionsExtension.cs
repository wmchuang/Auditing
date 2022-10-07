using Auditing.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Auditing.Storage.MongoDB;

public class MongoDBAuditingOptionsExtension : IAuditingOptionsExtension
{
    private readonly Action<MongoDBOptions> _configure;

    public MongoDBAuditingOptionsExtension(Action<MongoDBOptions> configure)
    {
        _configure = configure;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <param name="services"></param>
    public void AddServices(IServiceCollection services)
    {
        services.TryAddSingleton<IStorageInitializer, MongoDBStorageInitializer>();
        services.TryAddSingleton<IAuditingStore, MongoDBAuditingStore>();

        //Add MySqlOptions
        services.Configure(_configure);
        
        services.TryAddSingleton<IMongoClient>(x =>
        {
            var options = x.GetService<IOptions<MongoDBOptions>>().Value;
            return new MongoClient(options.DatabaseConnection);
        });
    } 
}