using EasyBilling.Models;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    static public class ControllerHelper
    {
        /// <summary>
        /// Получение словаря всех контроллеров где:
        /// key = Имя класса контроллера
        /// value = Локализированное имя контроллера
        /// </summary>
        /// <returns></returns>
        static public async Task<Dictionary<string, string>> GetCtrlsNamesAsync()
            => await Task.Run(()
                => Assembly.GetAssembly(typeof(CustomController)).GetTypes()
                .Where(type => type.IsSubclassOf(typeof(CustomController)))
                .Select(type =>
                {
                    var dNameAttr = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    return (type.Name, dNameAttr ?? "");
                }).ToDictionary(t => t.Item1, t => t.Item2));

        /// <summary>
        /// Получение списка разрешений дейсвтий по  заданному имени контроллера
        /// </summary>
        /// <param name="cntrlName"></param>
        /// <returns></returns>
        static public async Task<List<ActionRight>> GetActionsRightsAsync(string cntrlName)
            => await Task.Run(() =>
            {
                var className = $"EasyBilling.Controllers.{cntrlName}";
                var type = Type.GetType(className);
                var methods = type.GetMethods().Where(m =>
                    m.GetCustomAttributes()
                    .Any(a => a.GetType().Equals(typeof(HttpGetAttribute)) ||
                        a.GetType().Equals(typeof(HttpPostAttribute))))
                    .ToArray();
                var actRghts = methods.Select(a =>
                {
                    var dNameAttr = a.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    return new ActionRight(a.Name, dNameAttr ?? "", true);
                }).ToList();

                return actRghts;
            });
    }
}