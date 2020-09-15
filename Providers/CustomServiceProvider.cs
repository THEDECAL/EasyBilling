using EasyBilling.Services;
using Microsoft.Extensions.DependencyInjection;
using EasyBilling.Data;

namespace EasyBilling.Providers
{
    public static class CustomServiceProvider
    {
        public static void AddAccessRightsManager(this IServiceCollection services) =>
            services.AddTransient<AccessRightsManager>();

        public static void AddTariffRegulator(this IServiceCollection services) =>
            services.AddScoped<TariffRegulator>();

        public static void AddEmailSender(this IServiceCollection services) =>
            services.AddScoped<EmailSender>();

        public static void AddDbInitializer(this IServiceCollection services) => 
            services.AddSingleton<DbInitializer>();
    }
}
