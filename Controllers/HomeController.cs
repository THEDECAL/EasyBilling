using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using EasyBilling.Attributes;
using EasyBilling.Models;
using System.ComponentModel;
using EasyBilling.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using EasyBilling.Services;
using EasyBilling.ViewModels;

namespace EasyBilling.Controllers
{
    [Authorize]
    [NoShowInMenu]
    [DisplayName("Главная")]
    public class HomeController : Controller
    {
        private AccessRightsManager _rightsManager;
        public HomeController(AccessRightsManager rightsManager)
        {
            _rightsManager = rightsManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
            => await Task.Run(async () =>
            {
                var role = await _rightsManager.GetRoleAsync(User.Identity.Name);
                return RedirectToAction("Index", role.DefaultControllerName.Name.Replace("Controller",""));
            });

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
        }

        [HttpPost]
        [HttpGet]
        public IActionResult ErrorAccess()
        {
            ViewData["Title"] = "Отказано в доступе";
            return View();
        }
    }
}
