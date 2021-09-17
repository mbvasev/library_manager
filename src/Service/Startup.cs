using Movies.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Movies.Service.Middleware;
using System.Diagnostics.CodeAnalysis;
using Movies.Domain.Data;

namespace Movies.Service
{
    [ExcludeFromCodeCoverage]
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
            services.AddRazorPages();
            services.AddSingleton<DomainFacade>();
            services.AddDbContext<MoviesDbContext>();
            services.AddMvc();
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies API", Version = "v1" });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomExceptionHandling();
            ////if (env.IsDevelopment())
            ////{
            ////app.UseDeveloperExceptionPage();
            ////}
            ////else
            ////{
            ////    app.UseExceptionHandler("/Error");
            ////}

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                c.SwaggerEndpoint("v1/swagger.json", "Movies API V1");
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Movie}");
            });
        }
    }
}
