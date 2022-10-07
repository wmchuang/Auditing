using Auditing;
using Auditing.Manager;
using Auditing.Options;
using Auditing.Storage;
using Auditing.Storage.Logger;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditingServiceCollectionExtensions
    {
        public static void AddAuditing(this IServiceCollection services, Action<AuditingOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddHttpContextAccessor();
            services.AddTransient<IAuditingManager, AuditingManager>();
            services.AddTransient<AuditingMiddleware>();
            
            var options = new AuditingOptions();
            setupAction(options);
            if (!options.Extensions.Any())
                options.RegisterExtension(new LoggerAuditingOptionsExtension());

            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }

            services.Configure(setupAction);
            
            // 如果不用库存储，获取不到初始化的服务，则不用执行引导程序
            using var provider = services.BuildServiceProvider();
            var storageInitializer = provider.GetService<IStorageInitializer>();
            if (storageInitializer != null)
                services.AddHostedService<Bootstrapper>();
        }

        /// <summary>
        /// 使用审计服务
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAuditing(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuditingMiddleware>();
        }
    }
}