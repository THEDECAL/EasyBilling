using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Helpers;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Права доступа")]
    public class AccessRightsController : CustomController
    {
        public AccessRightsController(BillingDbContext dbContext, RoleManager<Models.Pocos.Role> roleManager, UserManager<IdentityUser> userManager, IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, userManager, scopeFactory)
        {
        }

        [HttpGet]
        [DisplayName("Список")]
        public override async Task<IActionResult> Index()
        {
            return await Task.Run(() =>
            {
                var dvm = new DataViewModel<AccessRight>(_scopeFactory,
                    settings: Settings,
                    urlPath: HttpContext.Request.Path,
                    includeFields: new string[] { nameof(AccessRight.Role), nameof(AccessRight.Controller) },
                    excludeFields: new string[] { nameof(AccessRight.RoleId) }
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

                var model = new AccessRight();
                if (id != null)
                {
                    model = await _dbContext.AccessRights
                        .Include(ar => ar.Role)
                        .Include(ar => ar.Controller)
                        .FirstOrDefaultAsync(ar => ar.Id.Equals(id));
                    if (model == null)
                        model = new AccessRight();
                    else
                        ViewData["ActionPage"] = nameof(Update);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [DisplayName(("Создать"))]
        [HttpPost]
        public async Task<IActionResult> Create(AccessRight obj, List<bool> rights)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.RoleId = (await _roleManager.FindByNameAsync(obj.Role.Name)).Id;
                obj.Role = null;
                obj.Controller = await _dbContext.ControllersNames
                    .FirstOrDefaultAsync(c => c.Name.Equals(obj.Controller.Name));
                obj.UpdateActionsRights(rights);
                await _dbContext.AccessRights.AddAsync(obj);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [DisplayName(("Изменить"))]
        [HttpPost]
        public async Task<IActionResult> Update(AccessRight obj, List<bool> rights)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.RoleId = (await _roleManager.FindByNameAsync(obj.Role.Name)).Id;
                obj.Role = null;
                obj.Controller = await _dbContext.ControllersNames
                    .FirstOrDefaultAsync(c => c.Name.Equals(obj.Controller.Name));
                obj.UpdateActionsRights(rights);
                await Task.Run(() =>
                {
                    _dbContext.AccessRights.Update(obj);
                    _dbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(obj.Id);
        }

        [DisplayName(("Удалить"))]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var obj = await _dbContext.AccessRights.FindAsync(id);
            await Task.Run(() =>
            {
                if (obj != null)
                {
                    _dbContext.AccessRights.Remove(obj);
                    _dbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(AccessRight obj)
        {
            TryValidateModel(obj);
            ModelState.Remove("Role.LocalizedName");
            ModelState.Remove("Role.DefaultControllerName.Name");
            var cntrlExist = await _dbContext.ControllersNames
                .AnyAsync(c => c.Name.Equals(obj.Controller.Name));
            if (!cntrlExist)
            { ModelState.AddModelError("ControllerName", "Выбранная страница не существует"); }
            var role = await _roleManager.FindByNameAsync(obj.Role.Name);
            if (role == null)
            { ModelState.AddModelError("Role", "Выбранная роль не существует"); }
            if (!ActionName.Equals(nameof(Update)))
            {
                var accessRightExisting = _dbContext.AccessRights.Include("Role")
                    .FirstOrDefault(ar => ar.Role.Name.Equals(obj.Role.Name) &&
                    ar.Controller.Name.Equals(obj.Controller.Name));
                if (accessRightExisting != null)
                { ModelState.AddModelError("", "Правило для этой роли и страницы уже есть, измените его"); }
            }
        }

        public async Task<IActionResult> CheckCntrlExist([NotNull] string controllerName)
            => Json(await _dbContext.ControllersNames
                .FirstOrDefaultAsync(c => c.Name.Equals(controllerName)));

        public async Task<IActionResult> CheckRoleExist([NotNull] string role)
        {
            var roleExisting = await _roleManager.FindByNameAsync(role);
            return Json((roleExisting != null)?true:false);
        }
    }
}
