using CapiControls.BLL.DTO.Base;
using System;

namespace CapiControls.BLL.DTO
{
    public class RegionDTO : BaseDTO<Guid>
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
