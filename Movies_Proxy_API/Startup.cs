using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Movies_Proxy_API.Helpers;
using Movies_Proxy_API.Models;
using Movies_Proxy_API.Repository;
using Movies_Proxy_API.Repository.Interfaces;
using Serilog;
using StackExchange.Profiling;

namespace Movies_Proxy_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
            services.AddControllers();
            services.AddMemoryCache();
            //services.AddTransient < ExceptionHandlingMiddleware>();

            services.AddSwaggerDocument();

            services.Configure<DatabaseSettings>(options => { Configuration.GetSection("DatabaseSettings").Bind(options); });
            services.AddSingleton<IDataRepository, DataRepository>();
            services.Configure<UserSettings>(options =>
            { Configuration.GetSection("UserSettings").Bind(options); });

            if (Configuration["UserSettings:EnableMiniProfiler"] == "True")
            {
                services.AddMiniProfiler(options =>
                {
                    options.RouteBasePath = Configuration["UserSettings:BasePath"] + "/miniprofiler";
                    options.IgnorePath("/swagger/");
                    options.IgnorePath("/miniprofiler");
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/api/error/");
            }

            app.UseHttpsRedirection();
            if (Configuration["UserSettings:EnableMiniProfiler"] == "True")
            {
                app.UseMiniProfiler();
            }
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseRouting();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
