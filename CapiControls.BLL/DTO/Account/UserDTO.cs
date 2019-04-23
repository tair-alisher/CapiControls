using CapiControls.BLL.DTO.Base;
using System;
using System.Collections.Generic;

namespace CapiControls.BLL.DTO.Account
{
    public class UserDTO : BaseDTO<Guid>
    {
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public List<RoleDTO> Roles { get; set; }
    }
}
