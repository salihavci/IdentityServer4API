using IdentityServer4API.Models;
using IdentityServer4API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4API
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
            services.AddLocalApiAuthentication();
            services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
            services.AddControllers();
            services.AddDbContext<AppIdentityDbContext>(opts => opts.UseSqlServer("name=ConnectionStrings:DefaultConnection").UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).EnableSensitiveDataLogging());
            var builder = services.AddIdentityServer(opts =>
            {
                opts.Events.RaiseSuccessEvents = true;
                opts.Events.RaiseFailureEvents = true;
                opts.Events.RaiseErrorEvents = true;
                opts.Events.RaiseInformationEvents = true;
                opts.EmitStaticAudienceClaim = true;
            }).AddInMemoryApiScopes(Config.ApiScopes).AddInMemoryApiResources(Config.ApiResources).AddInMemoryClients(Config.Clients).AddInMemoryIdentityResources(Config.IdentityResources);

            builder.AddResourceOwnerValidator<IdentityResourceOwnerPasswordValidator>();
            builder.AddDeveloperSigningCredential();
            services.AddAuthentication();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityServer4API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityServer4API v1"));
            }
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
