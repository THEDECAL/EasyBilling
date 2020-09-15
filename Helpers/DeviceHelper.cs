using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    public class DeviceHelper
    {
        [Flags]
        public enum DeviceType
        {
            Switch,
            MngSwitch,
            Router,
            Hub,
            WiFi,
            WiFiAp,
            Repeater,
            Antenna,
            Server,
            Pc,
            Monitor,
            Printer,
            Nic,
            Transceiver,
            Converter
        };
        [Flags]
        public enum DeviceState
        {
            InWork,
            OnRepair,
            OnStock,
            Disabled
        };
        static public async Task<Dictionary<string, string>> GetDeviceTypes()
         => await Task.Run(() => new Dictionary<string, string>()
         {
             {  DeviceType.Switch.ToString(), "Коммутатор" },
             {  DeviceType.MngSwitch.ToString(), "Управляемый коммутатор" },
             {  DeviceType.Router.ToString(), "Маршрутизатор" },
             {  DeviceType.Hub.ToString(), "Хаб" },
             {  DeviceType.WiFi.ToString(), "Wi-Fi Роутер" },
             {  DeviceType.WiFiAp.ToString(), "Wi-Fi Точка доступа" },
             {  DeviceType.Repeater.ToString(), "Повторитель" },
             {  DeviceType.Antenna.ToString(), "Антенна" },
             {  DeviceType.Server.ToString(), "Сервер" },
             {  DeviceType.Pc.ToString(), "Компьютер" },
             {  DeviceType.Monitor.ToString(), "Монитор" },
             {  DeviceType.Printer.ToString(), "Управляемый коммутатор" },
             {  DeviceType.Nic.ToString(), "Сетевая карта" },
             {  DeviceType.Transceiver.ToString(), "Трансивер" },
             {  DeviceType.Converter.ToString(), "Конвертер" },
         });

        static public async Task<Dictionary<string, string>> GetDeviceStates()
         => await Task.Run(() => new Dictionary<string, string>()
         {
             {  DeviceState.InWork.ToString(), "В работе" },
             {  DeviceState.OnRepair.ToString(), "На ремонте" },
             {  DeviceState.OnStock.ToString(), "На складе" },
             {  DeviceState.Disabled.ToString(), "Отключен" }
         });
    }
}