using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class Device
    {
        private DeviceType _type;
        private DeviceState _state;

        [DisplayName("#")]
        public int Id { get; set; }

        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Тип*")]
        [Required]
        [Remote(action: "CheckType", controller: "Device", ErrorMessage = "Выбранный тип устройства не существует")]
        public DeviceType Type { get => _type ?? new DeviceType(); set => _type = value; }

        [DisplayName("Состояние*")]
        [Required]
        [Remote(action: "CheckState", controller: "Device", ErrorMessage = "Выбранное состояние устройства не существует")]
        public DeviceState State { get => _state ?? new DeviceState(); set => _state = value; }

        [DisplayName("Брэнд*")]
        [Required]
        public string Brand { get; set; }

        [DisplayName("Модель*")]
        [Required]
        public string Model { get; set; }

        [DisplayName("Серийный номер*")]
        [Required]
        public string SerialNumber { get; set; }

        [DisplayName("MAC-адрес")]
        public string MAC { get; set; }

        [DisplayName("IP-адрес")]
        public string IP { get; set; }

        [DisplayName("Локация")]
        public string Location { get; set; }

        [DisplayName("Комментарии")]
        public string Comment { get; set; }

        public string CustomDeviceField1 { get; set; }
        public string CustomDeviceField2 { get; set; }
        public string CustomDeviceField3 { get; set; }

        [DisplayName("Дата создания")]
        public DateTime DateOfCreation { get; private set; } = DateTime.Now;

        [DisplayName("Дата обновления")]
        public DateTime? DateOfUpdate { get; set; }
    }
}