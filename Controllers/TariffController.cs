using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Тарифы")]
    public class TariffController : CustomController
    {
        public TariffController(BillingDbContext dbContext, RoleManager<Role> roleManager, UserManager<IdentityUser> userManager, IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, userManager, scopeFactory)
        {
        }

        [HttpGet]
        [DisplayName("Список")]
        public override async Task<IActionResult> Index()
        {
            return await Task.Run(() =>
            {
                var dvm = new DataViewModel<Tariff>(_scopeFactory,
                    urlPath: HttpContext.Request.Path,
                    settings: Settings,
                    excludeFields: new string[]
                    {
                        nameof(Tariff.DateOfCreation),
                        nameof(Tariff.DateOfUpdate)
                    }
                );

                return View("CustomIndex", model: dvm);
            });
        }

        [DisplayName(("Форма добавить/изменить"))]
        [HttpGet]
        public async Task<IActionResult> AddUpdateForm(int? id = null)
        {
            return await Task.Run(async () =>
            {
                ViewData["ActionPage"] = nameof(Create);

                var model = new Tariff();
                if (id != null)
                {
                    model = await _dbContext.Tariffs.FindAsync(id);
                    if (model == null)
                        model = new Tariff();
                    else
                        ViewData["ActionPage"] = nameof(Update);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [DisplayName("Создать")]
        [HttpPost]
        public async Task<IActionResult> Create(Tariff obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                await _dbContext.Tariffs.AddAsync(obj);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [DisplayName("Изменить")]
        [HttpPost]
        public async Task<IActionResult> Update(Tariff obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                var objExisting = await _dbContext.Tariffs
                    .FirstOrDefaultAsync(t => t.Id.Equals(obj.Id));

                await Task.Run(() =>
                {
                    _dbContext.Entry(objExisting).CurrentValues.SetValues(obj);
                    _dbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(obj.Id);
        }

        [DisplayName("Удалить")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id = null)
        {
            var obj = await _dbContext.Tariffs.FindAsync(id);
            await Task.Run(() =>
            {
                if (obj != null)
                {
                    _dbContext.Tariffs.Remove(obj);
                    _dbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(Tariff obj)
        {
            TryValidateModel(obj);

            var isTariffExist = await _dbContext.Tariffs
                .AnyAsync(t => t.Name.Equals(obj.Name));
            if (ActionName.Equals(nameof(Create)))
            {
                if (isTariffExist)
                { ModelState.AddModelError("Name", "Такое название тарифа уже существует, выберите другое"); }
            }
            else
            {
                var tariffExisting = await _dbContext.Tariffs
                    .FirstOrDefaultAsync(t => t.Id.Equals(obj.Id));
                if(!tariffExisting.Name.Equals(obj.Name) && isTariffExist)
                { ModelState.AddModelError("Name", "Такое название тарифа уже существует, выберите другое"); }
            }
        }
    }
}
