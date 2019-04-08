using System;
using System.Collections.Generic;

namespace CapiControls.Models.Local.Account
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
        }
    }
}
