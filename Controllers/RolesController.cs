using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using EasyBilling.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace EasyBilling.Controllers
{
    [DisplayName("Роли")]
    public class RolesController : CustomController
    {
        public RolesController(BillingDbContext dbContext, RoleManager<Role> roleManager, UserManager<IdentityUser> userManager, IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, userManager, scopeFactory)
        {
        }

        [HttpGet]
        [DisplayName("Список")]
        public override async Task<IActionResult> Index()
        {
            return await Task.Run(() =>
            {
                var dvm = new DataViewModel<Role>(_scopeFactory,
                    settings: Settings,
                    includeFields: new string[] { nameof(Role.DefaultControllerName) },
                    urlPath: HttpContext.Request.Path);

                return View("CustomIndex", model: dvm);
            });
        }

        [DisplayName(("Форма добавить/изменить"))]
        [HttpGet]
        public async Task<IActionResult> AddUpdateForm(string id = null)
        {
            return await Task.Run(() =>
            {
                ViewData["ActionPage"] = nameof(Create);

                var model = new Role();
                if (id != null)
                {
                    model = _dbContext.Roles
                        .Include(r => (r as Role).DefaultControllerName)
                        .FirstOrDefault(r => r.Id.Equals(id)) as Role;
                    if (model == null)
                        model = new Role();
                    else
                        ViewData["ActionPage"] = nameof(Update);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [DisplayName(("Создать"))]
        [HttpPost]
        public async Task<IActionResult> Create(Role obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.DefaultControllerName = await _dbContext.ControllersNames
                    .FirstOrDefaultAsync(c => c.Name.Equals(obj.DefaultControllerName.Name));
                obj.NormalizedName = obj.Name.ToUpper();
                await _dbContext.Roles.AddAsync(obj);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [DisplayName(("Изменить"))]
        [HttpPost]
        public async Task<IActionResult> Update(Role obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                var defaultControllerName = await _dbContext.ControllersNames
                    .FirstOrDefaultAsync(c => c.Name.Equals(obj.DefaultControllerName.Name));
                obj.DefaultControllerNameId = defaultControllerName.Id;

                var objExisting = await _dbContext.Roles
                    .FirstOrDefaultAsync(o => o.Id.Equals(obj.Id)) as Role;

                await Task.Run(() =>
                {
                    _dbContext.Entry(objExisting).CurrentValues.SetValues(obj);
                    _dbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(obj.Id);
        }

        [DisplayName(("Удалить"))]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var obj = await _dbContext.Roles.FindAsync(id);
            await Task.Run(() =>
            {
                if (obj != null)
                {
                    if (!_dbContext.UserRoles.Any(ur => ur.RoleId.Equals(id)))
                    {
                        _dbContext.Roles.Remove(obj);
                        _dbContext.SaveChanges();
                    }
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(Role obj)
        {
            TryValidateModel(obj);

            var checkingNamePattern = @"^[a-z]*$";
            if (!Regex.IsMatch(obj.Name, checkingNamePattern))
            {
                ModelState.AddModelError("Name", "Название должно быть маленькими латинскими буквами");
            }

            var isControllerExist = await _dbContext
                .ControllersNames.AnyAsync(c => c.Name.Equals(obj.DefaultControllerName.Name));
            if (!isControllerExist)
            {
                ModelState.AddModelError("DefaultControllerName", "Такой страницы не существует");
            }

            var isRoleExist = await _dbContext
                .Roles.AnyAsync(r => r.Name.Equals(obj.Name));
            if (ActionName.Equals(nameof(Create)) && isRoleExist)
            {
                ModelState.AddModelError("Name", "Такая роль уже существует");
            }
            else
            {
                var oldRole = await _dbContext
                    .Roles.FirstOrDefaultAsync(r => r.Name.Equals(obj.Id));
                if (oldRole.Name.Equals(obj.Name))
                {
                    ModelState.AddModelError("Name", "Такая роль уже существует");
                }
            }
        }
    }
}
