using CapiControls.DAL.Entities.Base;
using System;
using System.Collections.Generic;

namespace CapiControls.DAL.Entities.Account
{
    public class User : BaseEntity<Guid>
    {
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}
