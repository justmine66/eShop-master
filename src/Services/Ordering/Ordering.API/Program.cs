using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure;
using Microsoft.Extensions.Options;
using Ordering.API.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using IntegrationEventLogEF;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<OrderingContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var logger = services.GetService<ILogger<OrderingContextSeed>>();
                    var settings = services.GetService<IOptions<OrderingSettings>>();

                    new OrderingContextSeed()
                    .SeedAsync(context, env, settings, logger)
                    .Wait();
                })
                .MigrateDbContext<IntegrationEventLogContext>((_, __) => { })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseHealthChecks("/hc")
            .ConfigureLogging((hostingContext, builder) =>
            {
                builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            })
            .Build();
    }
}
