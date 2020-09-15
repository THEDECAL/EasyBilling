using EasyBilling.Data;
using EasyBilling.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace EasyBilling
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            try
            {
                var scope = host.Services.CreateScope();
                var dbInit = scope.ServiceProvider.GetRequiredService<DbInitializer>();
                var tariffRegulator = scope.ServiceProvider.GetRequiredService<TariffRegulator>();

                //Инициализация БД
                dbInit.InitializeAsync().Wait();

                //Запуск таймера проверки
                tariffRegulator.CheckProfilesForTariffExpiring(null);
                tariffRegulator.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.UseStartup<Startup>());
    }
}