using CapiControls.BLL.DTO.Base;
using System;

namespace CapiControls.BLL.DTO
{
    public class GroupDTO : BaseDTO<Guid>
    {
        public string Title { get; set; }
    }
}
