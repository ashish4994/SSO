using System.Collections.Generic;

using CreditOne.Microservices.BuildingBlocks.Common.Configuration;
using CreditOne.Microservices.BuildingBlocks.Common.Configuration.Swagger;
using CreditOne.Microservices.BuildingBlocks.ExceptionFilters;
using CreditOne.Microservices.BuildingBlocks.LoggingFilter;
using CreditOne.Microservices.BuildingBlocks.RequestValidationFilter.Factory;
using CreditOne.Microservices.BuildingBlocks.RequestValidationFilter.Filter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;

namespace CreditOne.Microservices.Sso.API
{
    public class Startup
    {
        #region Private Members

        private readonly List<SwaggerConfiguration> _swaggerConfiguration;

        #endregion

        #region Constructor

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _swaggerConfiguration = Configuration.GetSection(nameof(SwaggerConfiguration)).Get<List<SwaggerConfiguration>>();
        }

        #endregion

        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy-public",
                    builder => builder.AllowAnyOrigin()
                    .Build());
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(InputParametersValidationFilter));
                options.Filters.Add(typeof(LogRequestResponseFilter));
                options.Filters.Add(typeof(BaseExceptionFilter));
            }).AddControllersAsServices()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IValidatorFactory, ValidatorFactory>();

            services.AddApiVersioning();

            services.AddSwaggerGen(options =>
            {                
                options.IncludeXmlComments(SwaggerConfiguration.GetXmlCommentsPath("CreditOne.Microservices.Sso.API.xml"));

                foreach (var swaggerConfiguration in _swaggerConfiguration)
                {
                    options.SwaggerDoc(swaggerConfiguration.Version,
                        new Info
                        {
                            Version = swaggerConfiguration.Version,
                            Title = "Ping Federate SingleSignOn API",
                            Description = "Provides Ping Federate SingleSignOn Operations."
                        });
                }

                options.OperationFilter<CustomOperationFilter>();

                options.DocumentFilter<CustomDocumentFilter>();

                options.DocInclusionPredicate(SwaggerConfiguration.MapApiVersionAttributes());

                SwaggerConfiguration.SetBearerToken(options);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Test"))
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger(option =>
                {
                    option.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        swaggerDoc.Host = httpReq.Host.Value;
                        swaggerDoc.Schemes = new List<string>() { httpReq.Scheme };
                        swaggerDoc.BasePath = httpReq.PathBase.HasValue ? httpReq.PathBase.Value : "/";
                    });
                });

                app.UseSwaggerUI(option =>
                {
                    foreach (var swaggerConfiguration in _swaggerConfiguration)
                    {
                        option.SwaggerEndpoint(swaggerConfiguration.UIEndpoint, swaggerConfiguration.Version);
                    }
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("CorsPolicy-public");
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        #endregion
    }
}
