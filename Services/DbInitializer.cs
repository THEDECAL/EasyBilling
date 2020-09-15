using EasyBilling.Controllers;
using EasyBilling.Data;
using EasyBilling.Helpers;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Services
{
    public class DbInitializer
    {
        private readonly Random _random;
        private readonly BillingDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<Models.Pocos.Role> _roleManager;
        private const string _password = @"AQeT.5*gehWqeAh";

        public DbInitializer(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            _userManager = sp.GetRequiredService<UserManager<IdentityUser>>();
            _roleManager = sp.GetRequiredService<RoleManager<Models.Pocos.Role>>();
            _dbContext = sp.GetRequiredService<BillingDbContext>();
            _random = new Random();
        }
        public async Task InitializeAsync()
        {
            using (_dbContext)
            {
                await ControllersNamesInitAsync();
                await DeviceStatesInitAsync();
                await DeviceTypesInitAsync();
                await TariffsInitAsync();
                await RolesInitAsync();
                await AccessRightsInitAsync();
                await DevicesInitAsync();
                await UsersInitAsync();
                await ClientsInitAsync();
                await PaymentsInitAsync();
            }
        }

        /// <summary>
        /// Инициализация пользователей
        /// </summary>
        /// <returns></returns>
        private async Task UsersInitAsync()
        {
            if (_userManager.Users.Count() == 0)
            {
                Profile adminProfile = new Profile()
                {
                    FirstName = "Администратор",
                    SecondName = "Биллинга",
                    Address = "Пушкина 9-15",
                    Tariff = await _dbContext.Tariffs
                        .FirstOrDefaultAsync(t => t.Name.Equals("Коллегиальный")),
                    Account = new IdentityUser()
                    {
                        UserName = "admin",
                        Email = "admin@localhost",
                        PhoneNumber = "099-999-99-99",
                        EmailConfirmed = true,
                        LockoutEnabled = true
                    }
                };
                var result = await _userManager.CreateAsync(adminProfile.Account, _password);
                if (result.Succeeded)
                {
                    var adminRole = Helpers.Role.admin.ToString();
                    await _userManager.AddToRoleAsync(adminProfile.Account, adminRole);
                    await _dbContext.Profiles.AddAsync(adminProfile);
                    await _dbContext.SaveChangesAsync();
                }

                Profile operatorProfile = new Profile()
                {
                    FirstName = "Оператор",
                    SecondName = "Биллинга",
                    Address = "Пушкина 9-15",
                    Tariff = await _dbContext.Tariffs
                        .FirstOrDefaultAsync(t => t.Name.Equals("Коллегиальный")),
                    Account = new IdentityUser()
                    {
                        UserName = "operator",
                        Email = "operator@localhost",
                        PhoneNumber = "099-999-99-99",
                        EmailConfirmed = true,
                        LockoutEnabled = true
                    }
                };
                result = await _userManager.CreateAsync(operatorProfile.Account, _password);
                if (result.Succeeded)
                {
                    var operatorRole = Helpers.Role.@operator.ToString();
                    await _userManager.AddToRoleAsync(operatorProfile.Account, operatorRole);
                    await _dbContext.Profiles.AddAsync(operatorProfile);
                    await _dbContext.SaveChangesAsync();
                }

                Profile casherProfile = new Profile()
                {
                    FirstName = "Кассир",
                    SecondName = "Биллинга",
                    Address = "Пушкина 9-15",
                    Tariff = await _dbContext.Tariffs
                        .FirstOrDefaultAsync(t => t.Name.Equals("Коллегиальный")),
                    Account = new IdentityUser()
                    {
                        UserName = "casher",
                        Email = "casher@localhost",
                        PhoneNumber = "099-999-99-99",
                        EmailConfirmed = true,
                        LockoutEnabled = true
                    }
                };
                result = await _userManager.CreateAsync(casherProfile.Account, _password);
                if (result.Succeeded)
                {
                    var casherRole = Helpers.Role.casher.ToString();
                    await _userManager.AddToRoleAsync(casherProfile.Account, casherRole);
                    await _dbContext.Profiles.AddAsync(casherProfile);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Инициализация ролей
        /// </summary>
        /// <returns></returns>
        private async Task RolesInitAsync()
        {
            if (_roleManager.Roles.Count() == 0)
            {
                var dic = await RoleHelper.GetRolesAsync();

                foreach (var item in dic)
                {
                    var cntrlName = await _dbContext.ControllersNames
                        .FirstOrDefaultAsync(cn => cn.Name.Equals(item.Value[1]));

                    await _roleManager.CreateAsync(new Models.Pocos.Role()
                    {
                        Name = item.Key,
                        NormalizedName = item.Key.ToUpper(),
                        LocalizedName = item.Value[0],
                        DefaultControllerName = cntrlName
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Инициализация прав доступа
        /// </summary>
        /// <returns></returns>
        private async Task AccessRightsInitAsync()
        {
            if (_dbContext.AccessRights.Count() == 0)
            {
                var adminRole = await _roleManager
                    .FindByNameAsync(Helpers.Role.admin.ToString());
                var operatorRole = await _roleManager
                    .FindByNameAsync(Helpers.Role.@operator.ToString());
                var casherRole = await _roleManager
                    .FindByNameAsync(Helpers.Role.casher.ToString());
                var clientRole = await _roleManager
                    .FindByNameAsync(Helpers.Role.client.ToString());

                foreach (var item in _dbContext.ControllersNames.ToArray())
                {
                    var rights = await ControllerHelper
                        .GetActionsRightsAsync(item.Name);

                    if (item.Name.Equals(nameof(UsersController)))
                    {
                        await _dbContext.AccessRights
                            .AddAsync(new AccessRight()
                            {
                                Controller = item,
                                Role = operatorRole,
                                IsAvailable = true
                            });
                    }
                    else if (item.Name.Equals(nameof(CassaController)))
                    {
                        await _dbContext.AccessRights
                            .AddAsync(new AccessRight()
                            {
                                Controller = item,
                                Role = casherRole,
                                IsAvailable = true
                            });
                    }
                    else if (item.Name.Equals(nameof(ClientController)))
                    {
                        var cRights = await ControllerHelper
                            .GetActionsRightsAsync(item.Name);
                        var accr = new AccessRight()
                        {
                            Controller = item,
                            Role = clientRole,
                            IsAvailable = true
                        };
                        accr.UpdateActionRight(nameof(ClientController.Get), false);
                         await _dbContext.AccessRights.AddAsync(accr);
                    }

                    var ar = new AccessRight()
                    {
                        Controller = item,
                        Role = adminRole,
                        IsAvailable = true
                    };

                    await _dbContext.AccessRights.AddAsync(ar);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Инициализация списка имён контроллеров
        /// </summary>
        /// <returns></returns>
        private async Task ControllersNamesInitAsync()
        {
            if (_dbContext.ControllersNames.Count() == 0)
            {
                var dic = await ControllerHelper.GetCtrlsNamesAsync();
                foreach (var item in dic)
                {
                    await _dbContext.ControllersNames.AddAsync(new ControllerName()
                    {
                        Name = item.Key,
                        LocalizedName = item.Value
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Инициализация типов устройства
        /// </summary>
        /// <returns></returns>
        private async Task DeviceTypesInitAsync()
        {
            if (_dbContext.DeviceTypes.Count() == 0)
            {
                var dic = await DeviceHelper.GetDeviceTypes();
                foreach (var item in dic)
                {
                    await _dbContext.DeviceTypes.AddAsync(new DeviceType()
                    {
                        Name = item.Key,
                        LocalizedName = item.Value
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Инициализация состояний устройства
        /// </summary>
        /// <returns></returns>
        private async Task DeviceStatesInitAsync()
        {
            if (_dbContext.DeviceStates.Count() == 0)
            {
                var dic = await DeviceHelper.GetDeviceStates();
                foreach (var item in dic)
                {
                    await _dbContext.DeviceStates.AddAsync(new DeviceState()
                    {
                        Name = item.Key,
                        LocalizedName = item.Value
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Инициализация списка устройств
        /// </summary>
        /// <returns></returns>
        private async Task DevicesInitAsync()
        {
            if (_dbContext.Devices.Count() == 0)
            {
                var brandArray = new string[]
                { "HP", "Dell", "Samsung", "Sony", "Philips", "Supermicro", "LG", "Acer", "Asus", "Lenovo"};

                for (int i = 0; i < 100; i++)
                {
                    var model = await GenerateHelper.GetStringAsync(_random.Next(5, 11));
                    var sn = await GenerateHelper.GetStringAsync(_random.Next(10, 21), false, true, true);
                    var state = await _dbContext.DeviceStates.OrderBy(o => Guid.NewGuid()).FirstOrDefaultAsync();
                    var type = await _dbContext.DeviceTypes.OrderBy(o => Guid.NewGuid()).FirstOrDefaultAsync();
                    var name = type.LocalizedName + ' ' + GenerateHelper.GetString(_random.Next(1, 5), false, false, true);

                    var device = new Device()
                    {
                        Name = name,
                        Brand = brandArray[_random.Next(0, brandArray.Count())],
                        Model = $"{model}-{_random.Next(100, 999)}",
                        SerialNumber = sn,
                        State = state,
                        Type = type
                    };

                    await _dbContext.Devices.AddAsync(device);
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        ///  Инициалищация тарифов
        /// </summary>
        /// <returns></returns>
        private async Task TariffsInitAsync()
        {
            if (_dbContext.Tariffs.Count() == 0)
            {
                await _dbContext.Tariffs.AddAsync(new Tariff()
                {
                    Name = "Беспроводной",
                    Price = 90.0,
                    IsEnabled = true,
                    AmounfOfDays = 28,
                    AmountOfTraffic = 409600,
                    BandwidthInput = 25000,
                    BandwidthOutput = 25000
                });
                await _dbContext.Tariffs.AddAsync(new Tariff()
                {
                    Name = "Стандартный",
                    Price = 90.0,
                    IsEnabled = true,
                    AmounfOfDays = 28,
                    AmountOfTraffic = 0,
                    BandwidthInput = 50000,
                    BandwidthOutput = 50000
                });
                await _dbContext.Tariffs.AddAsync(new Tariff()
                {
                    Name = "Скоростной",
                    Price = 150.0,
                    IsEnabled = true,
                    AmounfOfDays = 28,
                    AmountOfTraffic = 0,
                    BandwidthInput = 100000,
                    BandwidthOutput = 100000
                });
                await _dbContext.Tariffs.AddAsync(new Tariff()
                {
                    Name = "Коллегиальный",
                    Price = 0,
                    IsEnabled = true,
                    IsPublish = false,
                    AmounfOfDays = 0,
                    AmountOfTraffic = 0,
                    BandwidthInput = 100000,
                    BandwidthOutput = 100000
                }); ;

                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        ///  Инициалищация базы данных абонентов
        /// </summary>
        /// <returns></returns>
        private async Task ClientsInitAsync()
        {
            if (_dbContext.Profiles.Count() == 3)
            {
                var streetsArray = new string[]
                    { "Харьковская", "Яворницкого", "Победы", "Донецкая", "Мира", "Соборная", "Центральная", "Тараса-Шевченко", "Петрова", "Лермонтова" };
                var fnamesArray = new string[]
                    { "Игорь", "Николай", "Иван", "Рудик", "Анатолий", "Екатерина", "Светлана", "Пётр", "Григорий", "Елена", "Анна", "Ирина", "Василий", "Роман", "Сергей", "Никита" };
                var snamesArray = new string[]
                    { "Щедренко", "Ивановко", "Выпренко", "Зоря", "Кальмуш", "Шпак", "Дыркач", "Соленко", "Головач", "Чёрный", "Косынко", "Лысенко", "Юдиненко", "Колаченко", "Строгач", "Карго", "Зинер", "Зиберт"};

                var clientRole = Helpers.Role.client.ToString();

                for (int i = 0; i < 10; i++)
                {
                    await Task.Run(async () =>
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            var username = $"abon{(i + 1) * (j + 1)}";
                            var email = await GenerateHelper.GetStringAsync(_random.Next(10, 16)) + "@gmail.com";
                            var fname = fnamesArray[_random.Next(0, fnamesArray.Count())];
                            var sname = snamesArray[_random.Next(0, snamesArray.Count())];
                            var address = $"{streetsArray[_random.Next(0, streetsArray.Count())]} {_random.Next(1, 100)}-{_random.Next(1, 100)}";
                            var tariff = await _dbContext.Tariffs.Where(t => t.IsPublish)
                                .OrderBy(o => Guid.NewGuid()).FirstOrDefaultAsync();

                            var profile = new Profile()
                            {
                                Account = new IdentityUser()
                                {
                                    UserName = username,
                                    Email = email,
                                    PhoneNumber = "099-999-99-99",
                                    EmailConfirmed = true,
                                    LockoutEnabled = true
                                },
                                FirstName = fname,
                                SecondName = sname,
                                Address = address,
                                Tariff = tariff
                            };

                            var result = await _userManager.CreateAsync(profile.Account, _password);
                            if (result.Succeeded)
                            {
                                _userManager.AddToRoleAsync(profile.Account, clientRole).Wait();
                                _dbContext.Profiles.Add(profile);
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                    });
                }
            }
        }

        /// <summary>
        /// Инициализация оплат
        /// </summary>
        /// <returns></returns>
        private async Task PaymentsInitAsync()
        {
            if (_dbContext.Payments.Count() == 0)
            {
                var casherRole = await _roleManager.FindByNameAsync("casher");
                var cahserUserRole = await _dbContext.UserRoles
                    .FirstOrDefaultAsync(ur => ur.RoleId.Equals(casherRole.Id));
                var casherProfile = await _dbContext.Profiles
                    .Include(p => p.Account)
                    .FirstOrDefaultAsync(p => p.Account.Id.Equals(cahserUserRole.UserId));

                for (int i = 0; i < 200; i++)
                {
                    var profileToPaymet = await _dbContext.Profiles
                        .Include(p => p.Tariff)
                        .Where(p => !p.Tariff.Name.Equals("Коллегиальный"))
                        .OrderBy(o => Guid.NewGuid())
                        .FirstOrDefaultAsync();

                    var payment = new Payment()
                    {
                        SourceProfile = casherProfile,
                        DestinationProfile = profileToPaymet,
                        Role = casherRole,
                        Amount = profileToPaymet.Tariff.Price + 10,
                        Comment = "Инициализатор"
                    };

                    profileToPaymet.AmountOfCash += payment.Amount;

                    await Task.Run(() =>
                    {
                        _dbContext.Update(profileToPaymet);
                        _dbContext.Add(payment);
                    });
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}