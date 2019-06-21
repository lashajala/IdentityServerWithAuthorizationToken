namespace PkceClient
{
    using System;
    using System.Collections.Generic;
    using IdentityModel;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public static IServiceCollection AddBearerTokenAuthentication(IServiceCollection services, IServiceProvider serviceProvider = null)
        {
            ILogger logger;
            if (serviceProvider == null)
            {
                serviceProvider = services.BuildServiceProvider();
            }

            logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AuthenticationExtension");
            logger.LogTrace("Starting AddBearerTokenAuthentication.");
            services
                .AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = "Bearer";
                        options.DefaultChallengeScheme = "Bearer";
                    })
                .AddIdentityServerAuthentication(
                    OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer,
                    settings =>
                    {
                        settings.Authority = "http://localhost:5000";
                        settings.ApiName = "demo_api_swagger";
                        settings.ApiSecret = "acf2ec6fb01a4b698ba240c2b10a0243";
                        settings.CacheDuration = TimeSpan.FromSeconds(10000);
                        settings.EnableCaching = true;
                        settings.RequireHttpsMetadata = false;
                        settings.SaveToken = true;
                        
                    });

            return services;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Protected API", Version = "v1" });
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "authorizationCode", // just get token via browser (suitable for swagger SPA)
                    AuthorizationUrl = "http://localhost:5000/connect/authorize",
                    TokenUrl = "http://localhost:5000/connect/token",
                    Scopes = new Dictionary<string, string> { { "demo_api_swagger", "Demo API - full access" } },
                    Type = "oauth2",
                    
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>(); // Required to use access token
            });

            AddBearerTokenAuthentication(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();

            // Swagger UI
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.RoutePrefix = string.Empty;
                options.OAuthClientId("demo_api_swagger");
                options.OAuthAppName("Demo API - Swagger"); // presentation purposes only
            });
        }
    }
}
