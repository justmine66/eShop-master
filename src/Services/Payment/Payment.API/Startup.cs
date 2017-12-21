using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventBusRabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Microsoft.Extensions.HealthChecks;
using EventBus.Abstractions;
using Autofac;
using EventBus;
using Autofac.Extensions.DependencyInjection;
using Payment.API.IntegrationEvents.Events;
using Payment.API.IntegrationEvents.EventHandling;

namespace Payment.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<PaymentSettings>(this.Configuration);

            services.AddSingleton<IRabbitMQPersistentConnection>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory() { HostName = this.Configuration["EventBusConnection"] };

                if (!string.IsNullOrWhiteSpace(this.Configuration["EventBusUserName"]))
                {
                    factory.UserName = this.Configuration["EventBusUserName"];
                }
                if (!string.IsNullOrWhiteSpace(this.Configuration["EventBusPassword"]))
                {
                    factory.Password = this.Configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            this.RegisterEventBus(services);

            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory)
        {
            var path_base = this.Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(path_base))
            {
                app.UsePathBase(path_base);
            }

            this.ConfigureEventBus(app);
        }

        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = Configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(provider =>
            {
                var connection = provider.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = provider.GetRequiredService<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                var scope = provider.GetRequiredService<ILifetimeScope>();
                var subManager = provider.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ.EventBusRabbitMQ(connection, logger, scope, subManager, subscriptionClientName, retryCount);
            });
        }
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
        }
    }
}
