using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_ASP.NET.ViewModels.Roles
{
    public class CreateViewModel
    {
        [Required]
        public string Rolename { get; set; }
        public string Description { get; set; }
    }
}
