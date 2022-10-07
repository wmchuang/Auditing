using Auditing.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Auditing.Storage.Mysql;

public class MySqlAuditingOptionsExtension : IAuditingOptionsExtension
{
    private readonly Action<MySqlOptions> _configure;

    public MySqlAuditingOptionsExtension(Action<MySqlOptions> configure)
    {
        _configure = configure;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <param name="services"></param>
    public void AddServices(IServiceCollection services)
    {
        services.TryAddSingleton<IStorageInitializer, MySqlStorageInitializer>();
        services.TryAddSingleton<IAuditingStore, MySqlAuditingStore>();

        //Add MySqlOptions
        services.Configure(_configure);
    } 
}