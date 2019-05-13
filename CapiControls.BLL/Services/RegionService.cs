using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Units;
using System.Collections.Generic;

namespace CapiControls.BLL.Services
{
    public class RegionService : BaseService, IRegionService
    {
        private readonly ILocalUnitOfWork _uow;

        public RegionService(ILocalUnitOfWork uow, IMapper mapper) : base(mapper)
        {
            _uow = uow;
        }

        public IEnumerable<RegionDTO> GetRegions()
        {
            return Mapper.Map<IEnumerable<Region>, IEnumerable<RegionDTO>>(_uow.RegionRepository.GetAll());
        }
    }
}
