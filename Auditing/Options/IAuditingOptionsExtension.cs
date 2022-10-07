using Microsoft.Extensions.DependencyInjection;

namespace Auditing.Options;

public interface IAuditingOptionsExtension
{
    /// <summary>
    /// Registered child service.
    /// </summary>
    /// <param name="services">add service to the <see cref="IServiceCollection" /></param>
    void AddServices(IServiceCollection services);
}