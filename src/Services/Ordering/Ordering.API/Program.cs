using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel()
            .UseStartup<Startup>()
            .UseHealthChecks("/hc")
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureLogging((hostingContext, builder) =>
            {
                builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            }).Build();
    }
}
