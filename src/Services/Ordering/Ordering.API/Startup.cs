using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.API.Infrastructure.Filters;
using Microsoft.Extensions.Hosting;
using Ordering.API.Infrastructure.HostedServices;
using Ordering.API.Application.IntegrationEvents;
using Microsoft.Extensions.HealthChecks;
using Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Ordering.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 注入MVC框架服务
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();

            // 注入更新管理服务
            services.AddSingleton<IHostedService, GracePeriodManagerService>();
            // 注入订单子域集成事件服务
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();
            // 注入健康检查服务
            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddSqlCheck("OrderingDb", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            });
            // 注入EFCore服务
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrderingContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString"],
                        sqlServerOptionsAction: (sqlOptions) =>
                         {
                             sqlOptions.MigrationsAssembly(typeof(Startup).GetType().Assembly.GetName().Name);
                             sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                         });
                });
            // 注入配置信息
            services.Configure<OrderingSettings>(Configuration);
            // 注入WebApi文档生成服务
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Version = "1.0",
                    Title = "eshop - Ordering Http Api",
                    Description = "订单子域微服务",
                    TermsOfService = "服务条款 "
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
