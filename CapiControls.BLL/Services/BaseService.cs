using AutoMapper;
using CapiControls.BLL.Interfaces;

namespace CapiControls.BLL.Services
{
    public class BaseService : IBaseService
    {
        protected readonly IMapper Mapper;

        public BaseService(IMapper mapper)
        {
            Mapper = mapper;
        }
    }
}
