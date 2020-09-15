using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class Tariff
    {
        [DisplayName("#")]
        public int Id { get; set; }

        [DisplayName("Название*")]
        //[Remote(action: "CheckName", controller: "Tariff", ErrorMessage = "Выбранное имя тарифа уже занято")]
        [Required(ErrorMessage = "Не ввведено название")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Длина строки от 5 до 30 символов")]
        public string Name { get; set; }

        [DisplayName("Цена")]
        public double Price { get; set; } = 0;

        [DisplayName("Объём трафика (Кбайт)")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Минимум 0 Кбайт")]
        public int AmountOfTraffic { get; set; } = 0;

        [DisplayName("Входящая*")]
        [Required(ErrorMessage = "Не указана входящая пропускная способность")]
        [Range(100, 100000, ErrorMessage = "Допустимая пропускная способность от 100 до 100000 Кбит/c")]
        public int BandwidthInput { get; set; } = 100000;

        [DisplayName("Исходящая*")]
        [Required(ErrorMessage = "Не указана исходящая пропускная способность")]
        [Range(100, 100000, ErrorMessage = "Допустимая пропускная способность от 100 до 100000 Кбит/c")]
        public int BandwidthOutput { get; set; } = 100000;

        [DisplayName("Количество дней")]
        [Range(0,31, ErrorMessage = "Допустимое количество дней от 0 до 31")]
        public int AmounfOfDays { get; set; }

        [DisplayName("Дата создания")]
        public DateTime DateOfCreation { get; private set; } = DateTime.Now;

        [DisplayName("Дата изменения")]
        public DateTime? DateOfUpdate { get; set; }

        [DisplayName("Отображать у клиента?")]
        public bool IsPublish { get; set; } = true;

        [DisplayName("Включен?")]
        public bool IsEnabled { get; set; } = true;

        public override string ToString() => this.Name;
    }
}