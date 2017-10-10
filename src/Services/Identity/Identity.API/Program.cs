using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;
using IdentityServer4.EntityFramework.DbContexts;
using Identity.API.Data;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Identity.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebhost(args)
                .MigrateDbContext<PersistedGrantDbContext>((_, __) => { })
                .MigrateDbContext<ApplicationDbContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var logger = services.GetService<ILogger<ApplicationDbContextSeed>>();
                    var settings = services.GetService<IOptions<AppSettings>>();

                    new ApplicationDbContextSeed()
                        .SeedAsync(context, env, logger, settings)
                        .Wait();
                })
                .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                {
                    var configuration = services.GetService<IConfiguration>();

                    new ConfigurationDbContextSeed()
                        .SeedAsync(context, configuration)
                        .Wait();
                });

            host.Run();
        }

        public static IWebHost BuildWebhost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseHealthChecks("/hc")
            .UseStartup<Startup>()
            .ConfigureLogging((hostingContex, loggingBuilder) =>
            {
                loggingBuilder.AddConfiguration(hostingContex.Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            })
            .Build();
    }
}
