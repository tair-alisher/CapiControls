using CapiControls.BLL.DTO.Base;
using System;

namespace CapiControls.BLL.DTO.Account
{
    public class RoleDTO : BaseDTO<Guid>
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
