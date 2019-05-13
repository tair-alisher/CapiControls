using CapiControls.DAL.Entities.Base;
using System;

namespace CapiControls.DAL.Entities.Account
{
    public class RawUserData : BaseEntity<Guid>
    {
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleTitle { get; set; }
    }
}
