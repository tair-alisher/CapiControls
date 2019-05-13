using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.DTO.Account;
using CapiControls.DAL.Entities;
using CapiControls.DAL.Entities.Account;

namespace CapiControls.Common.Mapping.Profiles
{
    public class BLLProfile : Profile
    {
        public BLLProfile()
        {
            CreateMap<UserDTO, User>(MemberList.None);
            CreateMap<RoleDTO, Role>(MemberList.None);

            CreateMap<GroupDTO, Group>(MemberList.None);
            CreateMap<QuestionnaireDTO, Questionnaire>(MemberList.None);
            CreateMap<RegionDTO, Region>(MemberList.None);

            CreateMap<QuestionDataDTO, QuestionData>(MemberList.None);
            CreateMap<InterviewDTO, Interview>(MemberList.None);
        }
    }
}
