using IdentityServer4API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace IdentityServer4API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    var appDbContext = sp.GetRequiredService<AppIdentityDbContext>();
                    appDbContext.Database.Migrate();
                    var userManager = sp.GetRequiredService<UserManager<AppUser>>();
                    if(!userManager.Users.Any())
                    {
                        userManager.CreateAsync(new AppUser() { UserName = "savci8", Email = "savci8@ford.com.tr", City = "Bursa" }, "Password12*").Wait();
                    }
                }

                host.Run();
                return 0;

            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
