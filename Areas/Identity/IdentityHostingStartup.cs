using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

[assembly: HostingStartup(typeof(EasyBilling.Areas.Identity.IdentityHostingStartup))]
namespace EasyBilling.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public async void Configure(IWebHostBuilder builder)
        {
            await Task.Run(() =>
            {
                builder.ConfigureServices((context, services) =>
                {
                    string currConn = "DefaultConnection";
                    var env = context.HostingEnvironment;
                    if (env.IsDevelopment())
                    {
                        var conn = context.Configuration.GetSection("CurrentUsingConnection").Value;
                        if (!string.IsNullOrWhiteSpace(conn))
                        { currConn = conn; }
                    }

                    services.AddDbContext<BillingDbContext>(options =>
                       options.UseSqlServer(context.Configuration
                       .GetConnectionString(currConn)));
                });
            });

            await Task.Run(() => builder.ConfigureServices((services) =>
                services.AddDefaultIdentity<IdentityUser>(
                    options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<Role>()
                    .AddEntityFrameworkStores<BillingDbContext>()));
        }
    }
}