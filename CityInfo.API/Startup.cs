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
using Swashbuckle.AspNetCore.Swagger;

namespace CityInfo.API
{
    public class Startup
    {
        //.net core 1 için
        
        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json",optional:true,reloadOnChange:true);

            Configuration = builder.Build();
        }
        

        //.net core 2 için
        /*
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        */

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter()));

            //CORS Configuration - browser permission
            services.AddCors();




            //.net'teki property'lerin baş harfi büyükse bile response'da küçük olarak gidiyor,  null yapılan property bunu önüne geçer
            //.AddJsonOptions(o => (
            //         if(o.SerializerSettings.ContractResolver != null)
            //{

//    var castedResolver = o.SerializerSettings.ContractResolver
//        as DefaultContractResolver;
//    castedResolver.NamingStrategy = null;

//}));

#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<>(IMailService, CloudMailService);
#endif

            string connectionstring = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionstring));

            //repo servisi burada ekleriz.
            services.AddScoped<ICityInfoRepository, CityInfoRepository>();

            //Swagger Doküman tanımı
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            CityInfoContext cityInfoContext)
        {

            loggerFactory.AddConsole();

            loggerFactory.AddDebug();

            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            loggerFactory.AddNLog(); //üstteki yerine kullanılabilir.


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            //Cors Config - her şeye izin verildi.
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());


            cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            //Json çıktı verecek olan middleware'ı ekler.
            app.UseSwagger();

            app.UseSwaggerUI(c => {

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

            }

                );

            //automapper ile entity'mizi model'e mapledik. source -> entities.city, destination -> models.city...
            //bu demek oluyor ki, biz onu controller'da kullanabiliriz. 
            //it'll map property names on the source object to the same property names on the destination object
            //default olarak, no reference exceptionlarını ignore'lar from source to target
            //eğer bir property doesnt exist ise, it'll be ignored.
            
            AutoMapper.Mapper.Initialize( cfg =>
            {
                cfg.CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
                cfg.CreateMap<Entities.City, Models.CityDto>();
                cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
                cfg.CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>();
                cfg.CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
                cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>();
            });


            app.UseMvc();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
