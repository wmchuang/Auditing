using Auditing.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Auditing.Storage.Logger;

public class LoggerAuditingOptionsExtension : IAuditingOptionsExtension
{
    public void AddServices(IServiceCollection services)
    {
        services.TryAddSingleton<IAuditingStore, SimpleLogAuditingStore>();
    }
}