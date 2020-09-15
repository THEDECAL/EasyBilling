using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Services;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    [Authorize]
    [CheckAccessRights]
    public abstract class CustomController : Controller
    {
        private ControlPanelSettings _settings = new ControlPanelSettings();
        protected readonly BillingDbContext _dbContext;
        protected readonly RoleManager<EasyBilling.Models.Pocos.Role> _roleManager;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly IServiceScopeFactory _scopeFactory;

        public string DisplayName { get => (GetType().GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName; }
        public string ControllerName { get => GetType().Name.Replace("Controller", ""); }
        public string ActionName { get => RouteData.Values["action"] as string; }
        public IRequestCookieCollection Cookie { get => HttpContext.Request.Cookies; }
        public ControlPanelSettings Settings { get => _settings; private set => _settings = value ?? _settings; }

        public CustomController(BillingDbContext dbContext,
                RoleManager<EasyBilling.Models.Pocos.Role> roleManager,
                UserManager<IdentityUser> userManager,
                IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _scopeFactory = scopeFactory;
            var scope = _scopeFactory.CreateScope();
            scope.ServiceProvider.GetService<ILoggerFactory>().AddProvider(new DatabaseLoggerProvider());
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index()
            => await Task.Run(() => View());

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string actionDisplayName = "";
            try
            {
                var actionDnAtt = ControllerContext.ActionDescriptor.MethodInfo
                    .GetCustomAttribute<DisplayNameAttribute>();
                actionDisplayName = " - " + actionDnAtt?.DisplayName;
            }
            catch (Exception)
            { }

            ViewData["Title"] = $"{DisplayName}{actionDisplayName}";
            ViewData["ControllerName"] = ControllerName;
            ViewData["ActionName"] = ActionName;

            var settingsKey = ControllerName + "Settings";
            string settingsJson = null;
            HttpContext.Request.Cookies.TryGetValue(settingsKey, out settingsJson);

            if (settingsJson != null)
            {
                Settings = JsonConvert.DeserializeObject<ControlPanelSettings>(settingsJson);
            }
            else
            {
                var options = new CookieOptions()
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(1),
                        MaxAge = TimeSpan.FromDays(7),
                        SameSite = SameSiteMode.Lax,
                        IsEssential = true,
                        Secure = true
                    };

                HttpContext.Response.Cookies
                    .Append(settingsKey, JsonConvert.SerializeObject(Settings), options);
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}