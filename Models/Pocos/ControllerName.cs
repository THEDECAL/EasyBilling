using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class ControllerName
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";
        public string LocalizedName { get; set; } = "";

        public override string ToString() => (string.IsNullOrWhiteSpace(LocalizedName)) ? Name : LocalizedName;
    }
}
