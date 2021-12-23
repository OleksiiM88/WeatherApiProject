using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using WeatherApiProject.Interfaces;
using WeatherApiProject.Models;
using WeatherApiProject.Services;

namespace WebApiProject
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
            services.AddOptions();
            var section = Configuration.GetSection("OpenWeatherMapSettings");
            services.Configure<OpenWeatherMapSettings>(section);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherApiProject", Version = "v1" });
                c.ResolveConflictingActions(u => u.FirstOrDefault());
            });

            services.AddHttpClient("Default", a =>
            {
                a.Timeout = TimeSpan.FromSeconds(3);
                a.BaseAddress = new Uri("https://api.openweathermap.org/");
            });
            //services.AddHttpClient();
            services.AddScoped<IWeatherDataService, WeatherDataService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "WeatherApiProject v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
