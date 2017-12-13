using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBusRabbitMQ;
using Locations.Api.Infrastructure;
using Locations.Api.Infrastructure.Filters;
using Locations.Api.Infrastructure.Middlewares;
using Locations.Api.Infrastructure.Respositorys;
using Locations.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Locations.Api
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
            services.AddMvc(opts =>
            {
                opts.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();

            this.ConfigureAuthService(services);

            services.Configure<LocationSettings>(Configuration);

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "eShop - Location HTTP API",
                    Version = "v1",
                    Description = "The Location Microservice HTTP API. This is a Data-Driven/CRUD microservice using MongoDB",
                    TermsOfService = "Terms Of Service"
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize",
                    TokenUrl = $"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token",
                    Scopes = new Dictionary<string, string>()
                    {
                        { "locations", "Locations API" }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();

            });

            services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .Build());
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ILocationsService, LocationsService>();
            services.AddTransient<ILocationsRepository, LocationsRepository>();

            //configure autofac
            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            this.ConfigureAuth(app);

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Locations.API V1");
                  c.ConfigureOAuth2("locationsswaggerui", "", "", "Locations Swagger UI");
              });

            LocationsContextSeed.SeedAsync(app, loggerFactory)
                .Wait();
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(shareOpts =>
            {
                shareOpts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                shareOpts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opts =>
            {
                opts.Authority = this.Configuration["IdentityUrl"];
                opts.Audience = "locations";
                opts.RequireHttpsMetadata = false;
            });
        }

        private void ConfigureAuth(IApplicationBuilder app)
        {
            if (this.Configuration.GetValue<bool>("UserLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            app.UseAuthentication();
        }
    }
}
