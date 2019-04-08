using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapiControls.Models.Local.Account
{
    public class User
    {
        public Guid Id { get; set; }
        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Display(Name = "Имя")]
        public string UserName { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Роли")]
        public List<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
        }
    }
}
