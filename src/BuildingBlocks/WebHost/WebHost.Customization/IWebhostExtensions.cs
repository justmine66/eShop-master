using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// WebHost 扩展类
    /// </summary>
    public static class IWebhostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation($"正在迁移数据库，关联上下文{typeof(TContext)}");

                    context.Database
                        .Migrate();
                    seeder(context, services);

                    logger.LogInformation($"已经迁移数据库，关联上下文{typeof(TContext)}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"迁移数据库出现异常，关联上下文{typeof(TContext)}");
                }
            }

            return webHost;
        }
    }
}
