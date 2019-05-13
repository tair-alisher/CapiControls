using CapiControls.BLL.DTO;
using System.Collections.Generic;

namespace CapiControls.BLL.Interfaces
{
    public interface IRegionService : IBaseService
    {
        IEnumerable<RegionDTO> GetRegions();
    }
}
