using System;

namespace CapiControls.Models.Local.Account
{
    public class RawUserData
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleTitle { get; set; }
    }
}
