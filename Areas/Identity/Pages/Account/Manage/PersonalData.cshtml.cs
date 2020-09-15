using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EasyBilling.Areas.Identity.Pages.Account.Manage
{
    public partial class PersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BillingDbContext _dbContext;

        public PersonalDataModel(
            UserManager<IdentityUser> userManager,
            BillingDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Имя")]
            public string FName { get; set; }

            [Required]
            [Display(Name = "Фамилия")]
            public string SName { get; set; }

            [Display(Name = "Отчество")]
            public string Patronymic { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var profile = await _dbContext.Profiles
                .Include(p => p.Account)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Account.Id.Equals(user.Id));

            Input = new InputModel
            {
                FName = profile.FirstName,
                SName = profile.SecondName,
                Patronymic = profile.Patronymic
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //var user = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound($"Не могу загрузить аккаунт с ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Не могу загрузить аккаунт с ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var profile = await _dbContext.Profiles
                .Include(p => p.Account)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Account.Id.Equals(user.Id));

            profile.FirstName = Input.FName;
            profile.SecondName = Input.SName;
            profile.Patronymic = Input.Patronymic;

            _dbContext.Update(profile);
            await _dbContext.SaveChangesAsync();

            StatusMessage = "Ваша персональная информация успешно изменена.";
            return RedirectToPage();
        }
    }
}