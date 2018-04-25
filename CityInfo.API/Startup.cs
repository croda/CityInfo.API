using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
//using Newtonsoft.Json.Serialization;

namespace CityInfo.API
{
    public class Startup
    {
       // public static IConfigurationRoot Configuration;
       public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appSettings.json", optional:false, reloadOnChange:true);
            //    .AddJsonFile("appSettings.{env.EnvironmentName}.json", optional:true, reloadOnchange:true)
            //    .AddEnvironmentVariables();

            //Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter()));
            //.AddJsonOptions(o =>
            //{     if we want to alter the default naming of the JSON parameters
            //    if (o.SerializerSettings.ContractResolver != null)
            //    {
            //        var castedResolver = o.SerializerSettings.ContractResolver
            //            as DefaultContractResolver;
            //        castedResolver.NamingStrategy = null;
            //    }
            //});
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else

            services.AddTransient<IMailService, CloudMailService>();
#endif
            services.AddDbContext<CityInfoContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("cityInfoDBConnectionString")));

            services.AddScoped<ICityInfoRepository, CityInfoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            CityInfoContext cityInfoContext)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<CityInfoContext>().Database.Migrate();
                    serviceScope.ServiceProvider.GetService<CityInfoContext>().EnsureSeedDataForContext();
                }
            }

            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(config =>
            {
                config.CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
                config.CreateMap<Entities.City, Models.CityDto>();
                config.CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
                config.CreateMap<Models.PointOfInterestCreationDto, Entities.PointOfInterest>();
                config.CreateMap<Models.PointOfInterestUpdateDto, Entities.PointOfInterest>();
                config.CreateMap<Entities.PointOfInterest, Models.PointOfInterestUpdateDto>();
            });

            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
