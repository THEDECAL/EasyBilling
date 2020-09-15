using EasyBilling.Attributes;
using EasyBilling.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class AccessRight
    {
        private List<ActionRight> _rights = new List<ActionRight>();
        private Role _role = new Role();
        private ControllerName _controller = new ControllerName();
        private string _actionsRightsJson = "";

        [DisplayName("#")]
        public int Id { get; set; }

        public string RoleId { get; set; }
        [Required(ErrorMessage = "Не выбрана роль")]
        [DisplayName("Роль*")]
        [Remote(action: "CheckRoleExist", controller: "AccessRights", ErrorMessage = "Выбранная роль не существует")]
        public Role Role { get => _role; set => _role = value; }

        [Required(ErrorMessage = "Не выбрано название страницы")]
        [Remote(action: "CheckCntrlExist", controller: "AccessRights", ErrorMessage = "Выбранная страница не существует")]
        [DisplayName("Название страницы*")]
        public ControllerName Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                InitActionRightsAsync().Wait();
            }
        }

        [Required(ErrorMessage = "Не выбрано разрешение")]
        [DisplayName("Разрешение*")]
        public bool IsAvailable { get; set; }

        [Required]
        [NoShowInTable]
        public string ActionsRightsJson
        {
            get => _actionsRightsJson;
            set
            {
                _actionsRightsJson = value;
                if (!string.IsNullOrWhiteSpace(_actionsRightsJson))
                {
                    DeserializeRights();
                }
            }
        }

        [DisplayName("Действия")]
        [NoShowInTable]
        [NotMapped]
        public List<ActionRight> Rights
        {
            get
            {
                if (_rights.Count() == 0 &&
                    !string.IsNullOrWhiteSpace(_actionsRightsJson))
                {
                    DeserializeRights();
                }
                return _rights;
            }
        }

        [DisplayName("Разрешенные действия")]
        [NotMapped]
        public string RigthsString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in Rights)
                {
                    if (item.IsAvailable)
                    {
                       sb.Append(item.ToString());
                    }
                }
                return sb.ToString();
            }
        }

        private async Task InitActionRightsAsync()
        {
            if (!string.IsNullOrWhiteSpace(_controller.Name) &&
                string.IsNullOrWhiteSpace(_actionsRightsJson))
            {
                var rights = await ControllerHelper.GetActionsRightsAsync(_controller.Name);
                if (rights != null)
                {
                    _rights = rights;
                }
                else throw new NullReferenceException();
                SerializeRights();
            }
            else
            {
                DeserializeRights();
            }
        }

        private void SerializeRights()
        {
            try
            {
                _actionsRightsJson = JsonConvert.SerializeObject(_rights);
            }
            catch (JsonException ex)
            { Console.WriteLine(ex.StackTrace); }
        }

        private void DeserializeRights()
        {
            try
            {
                var rights = JsonConvert.DeserializeObject
                    <List<ActionRight>>(_actionsRightsJson);

                if (rights != null)
                {
                    _rights = rights;
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.StackTrace); }
        }

        private ActionRight GetActionRight(string actionName)
        {
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return _rights.FirstOrDefault(r => r.Name.Equals(actionName));
            }
            throw new ArgumentNullException();
        }

        public void UpdateActionRight(string actionName, bool isAvailable)
        {
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                var right = GetActionRight(actionName);
                right.IsAvailable = isAvailable;
                SerializeRights();
            }
            else throw new ArgumentNullException();
        }

        public void UpdateActionsRights(List<bool> listAvailables)
        {
            if (listAvailables != null)
            {
                for (int i = 0; i < Rights.Count(); i++)
                {
                    try
                    {
                        Rights[i].IsAvailable = listAvailables[i];
                    }
                    catch (Exception)
                    { }
                }
                SerializeRights();
            }
            else throw new ArgumentNullException();
        }
    }
}