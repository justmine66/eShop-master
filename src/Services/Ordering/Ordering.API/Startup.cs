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
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Ordering.API.Infrastructure.Services;
using System.Data.Common;
using IntegrationEventLogEF.Services;
using EventBusRabbitMQ;
using RabbitMQ.Client;
using EventBus.Abstractions;
using EventBus;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ordering.API.Infrastructure.AutofacModules;
using System.Data.SqlClient;
using Polly;
using Ordering.API.Infrastructure;
using System.Reflection;
using IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Ordering.API.Application.IntegrationEvents.Events;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // 注册MVC框架服务
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();
            // 注册更新管理服务
            services.AddSingleton<IHostedService, GracePeriodManagerService>();
            // 注册订单子域事件溯源服务
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(sp => conn => new IntegrationEventLogService(conn));
            services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();
            // 注册健康检查服务
            services.AddHealthChecks(checks =>
            {
                var minutes = 1;
                if (int.TryParse(Configuration["HealthCheck:Timeout"], out var minutesParsed))
                {
                    minutes = minutesParsed;
                }
                checks.AddSqlCheck("OrderingDb", Configuration["ConnectionString"], TimeSpan.FromMinutes(minutes));
            });
            // 注册EFCore SqlServer服务
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrderingContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                         {
                             sqlOptions.MigrationsAssembly("Ordering.API");
                             sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                         });
                })
                .AddDbContext<IntegrationEventLogContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionString"],
                         sqlServerOptionsAction: sqlOptions =>
                         {
                             sqlOptions.MigrationsAssembly("Ordering.API");
                             sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                         });
                });

            // 注册配置服务
            services.Configure<OrderingSettings>(Configuration);
            services.AddOptions();
            // 注册WebApi文档生成服务
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();

                options.SwaggerDoc("v1", new Info()
                {
                    Version = "1.0",
                    Title = "eshop - Ordering Http Api",
                    Description = "订单子域微服务",
                    TermsOfService = "服务条款 "
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme()
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/authorize",
                    TokenUrl = $"{Configuration.GetValue<string>("IdentityUrl")}/connect/authorize",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "Orders","Ordering API"}
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            // 注册跨域资源共享服务
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            // 注册身份服务
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            this.ConfigureAuthServices(services);
            // 注册事件总线服务
            // 001 事件总线连接器服务
            if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
            }
            else//默认注册基于RabbitMQ的事件总线连接器
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = Configuration["EventBusConnection"]
                    };

                    if (!string.IsNullOrEmpty(Configuration["EventBusName"]))
                    {
                        factory.UserName = Configuration["EventBusName"];
                    }

                    if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                    {
                        factory.Password = Configuration["EventBusPassword"];
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger);
                });
            }
            // 002 事件总线服务
            this.RegisterEventBusServices(services);

            // 配置 autofac Ioc 容器
            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrWhiteSpace(pathBase))
            {
                loggerFactory.CreateLogger("init").LogDebug($"Using PATH BASE '{pathBase}'");
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
            // 使用WebApi文档生成服务
            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "订单子域微服务 v1");
                    options.ConfigureOAuth2("orderingswaggerui", "", "", "Ordering Swagger UI");
                });
            // 配置事件总线，添加事件订阅信息。
            ConfigEventBus(app);
        }

        //配置事件总线
        private void ConfigEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
            eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>>();
            eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>>();
            eventBus.Subscribe<OrderStockRejectedIntegrationEvent, IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>>();
            eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>>();
            eventBus.Subscribe<OrderPaymentSuccededIntegrationEvent, IIntegrationEventHandler<OrderPaymentSuccededIntegrationEvent>>();
        }
        private void RegisterEventBusServices(IServiceCollection services)
        {
            if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
            }
            else//默认注册基于RabbitMQ的事件总线
            {
                services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>();
            }
            //事件总线-订阅管理器服务
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
        private void ConfigureAuthServices(IServiceCollection services)
        {
            //清空已有的声明
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;//开发环境中禁用Https
                options.Audience = "orders";
            });
        }
    }
}
