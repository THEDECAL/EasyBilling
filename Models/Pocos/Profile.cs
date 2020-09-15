using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyBilling.Models.Pocos
{
    public class Profile
    {
        private IdentityUser _account;
        private Tariff _tariff;

        [DisplayName("#")]
        public int Id { get; set; }

        [DisplayName("Логин")]
        [Required]
        public IdentityUser Account { get => _account ?? new IdentityUser(); set => _account = value; }

        [DisplayName("Имя*")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Фамилия*")]
        [Required]
        public string SecondName { get; set; }

        [DisplayName("Отчество")]
        public string Patronymic { get; set; }

        [DisplayName("Адрес подключения*")]
        [Required]
        public string Address { get; set; }

        [DisplayName("Счёт")]
        [Required]
        public double AmountOfCash { get; set; } = 0;

        [DisplayName("Тариф*")]
        [Required]
        //[Remote(action: "CheckTariff", controller: "Users", ErrorMessage = "Выбранный тариф не существует")]
        public Tariff Tariff { get => _tariff ?? new Tariff(); set => _tariff = value; }

        [DisplayName("Дата начала использования тарифа")]
        public DateTime? DateBeginOfUseOfTarrif { get; set; }

        [DisplayName("Использованный трафик")]
        public int UsedTraffic { get; set; } = 0;

        public string CustomProfileField1 { get; set; }
        public string CustomProfileField2 { get; set; }
        public string CustomProfileField3 { get; set; }

        [DisplayName("Дата создания")] 
        public DateTime DateOfCreation { get; set; } = DateTime.Now;

        [DisplayName("Дата обновления")]
        public DateTime? DateOfUpdate { get; set; }

        [DisplayName("Дата последнего входа")]
        public DateTime LastLogin { get; set; }

        [DisplayName("Заморожен?")]
        [Required]
        public bool IsHolded { get; set; } = false;

        [DisplayName("Включен?")]
        [Required]
        public bool IsEnabled { get; set; } = true;

        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        public override string ToString()
            => $"{this.FirstName} {this.SecondName} {this.Patronymic} {(!string.IsNullOrWhiteSpace(Account?.UserName) ? "(" + Account.UserName + ")" : "")}";
    }
}