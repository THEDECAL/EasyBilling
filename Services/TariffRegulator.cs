using EasyBilling.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EasyBilling.Models.Pocos;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Runtime.CompilerServices;

namespace EasyBilling.Services
{
    public class TariffRegulator
    {
        const string FILE_CONFIG_NAME = "Settings\\tariffRegulator.json";
        private BillingDbContext _dbContext;

        public TariffRegulator(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            _dbContext = sp.GetRequiredService<BillingDbContext>();
        }

        /// <summary>
        /// Провести автоматическую оплату по тарифу
        /// </summary>
        /// <param name="profile">Принимает экземпляр профиля</param>
        /// <returns></returns>
        private async Task CheckTariffStatehAsync(Profile profile)
        {
            if (profile != null && profile.Tariff != null)
            {
                var currDate = DateTime.Now;
                DateTime? expiryDate = null;

                if (profile.DateBeginOfUseOfTarrif != null)
                {
                    expiryDate = profile.DateBeginOfUseOfTarrif.Value
                        .AddDays(profile.Tariff.AmounfOfDays);
                }

                if (expiryDate == null || //Если тариф ни разу не использовался
                    (profile.Tariff.AmountOfTraffic != 0 &&
                    profile.UsedTraffic >= profile.Tariff.AmountOfTraffic &&
                    currDate < expiryDate) || //Если тариф ограниченный объёмом трафика и срок действия не подошел к концу
                    currDate >= expiryDate) //Если тариф уже окончил свой строк действия
                {
                    if (profile.AmountOfCash < profile.Tariff.Price)
                    {
                        profile.DateBeginOfUseOfTarrif = null;
                        profile.IsHolded = true;
                    }
                    else
                    {
                        profile.DateBeginOfUseOfTarrif = currDate;
                        profile.AmountOfCash -= profile.Tariff.Price;
                        await _dbContext.Payments.AddAsync(new Payment()
                        {
                            DestinationProfileId = profile.Id,
                            Comment = "Автоматический вычет тарифа",
                            Amount = -profile.Tariff.Price
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Проверка срока действия тарифа
        /// </summary>
        /// <param name="isTimer">Признак запуска таймера</param>
        public void CheckProfilesForTariffExpiring(object isTimer = null)
        {
            using (_dbContext)
            {
                var iQuery = _dbContext.Profiles
                    .Include(p => p.Tariff)
                    .Where(p => p.Tariff.AmounfOfDays != 0 &&
                        !p.IsHolded && p.IsEnabled);

                var currDate = DateTime.Now;
                iQuery.ForEachAsync(async p => 
                {
                    try
                    {
                        await CheckTariffStatehAsync(p);
                        _dbContext.Update(p);
                    }
                    catch (Exception ex)
                    { }
                }).Wait();

                try
                {
                    _dbContext.SaveChanges();
                }
                catch(Exception ex)
                { }
            }

            if (isTimer != null)
                Console.WriteLine($"Tariff regulator by timer is success run. ({DateTime.Now})");
        }

        /// <summary>
        /// Вычет суммы за тариф
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void StartToUseOfTariff(Profile profile)
        {
            using (_dbContext)
            {
                if (profile != null &&
                    !profile.IsHolded && profile.IsEnabled &&
                    profile.Tariff.AmounfOfDays != 0)
                {
                    CheckTariffStatehAsync(profile).Wait();
                    _dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Запуск таймера проверки срока действия тарифа
        /// </summary>
        public void Run()
        {
            try
            {
                var hours = 2; var minutes = 2;
                try
                {
                    var config = new ConfigurationBuilder().AddJsonFile(FILE_CONFIG_NAME).Build();
                    var hrs = config["RunningTimeHours"];
                    var mnts = config["RunningTimeMintutes"];

                    hours = int.Parse(hrs);
                    minutes = int.Parse(mnts);
                }
                catch (Exception ex)
                { Console.WriteLine(ex.StackTrace); }

                var currDate = DateTime.Now;
                var nextDay = currDate.AddDays(1);
                var firstRunDate = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, hours, minutes, 0, 0);
                var firstRunMls = firstRunDate.ToUniversalTime()
                    .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                var currMls = currDate.ToUniversalTime()
                    .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                var runThrough = (int)(firstRunMls - currMls) * 1000;

                int oneDay = 24 * 60 * 60 * 1000;
                TimerCallback tm = new TimerCallback(CheckProfilesForTariffExpiring);
                Timer timer = new Timer(tm, new object(), runThrough, oneDay);

                Console.WriteLine("Tariff regulator is success run.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error. Tariff regulator timer is not run!");
                Console.WriteLine(ex.Message);
            }
        }
    }
}