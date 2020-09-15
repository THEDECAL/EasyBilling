using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class Role : IdentityRole
    {
        private ControllerName _defaultControllerName = new ControllerName();

        public int DefaultControllerNameId { get; set; }

        [DisplayName("Страница по умолчанию*")]
        [Required(ErrorMessage = "Страницу обязательно нужно указать")]
        public ControllerName DefaultControllerName { get => _defaultControllerName; set => _defaultControllerName = value; }

        [DisplayName("Локализированное имя*")]
        [Required(ErrorMessage = "Локализированное имя обязательно нужно указать")]
        public string LocalizedName { get; set; }

        public override string ToString() => (string.IsNullOrWhiteSpace(LocalizedName)) ? Name : LocalizedName;
    }
}
