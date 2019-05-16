using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Interfaces.Units;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapiControls.BLL.Services
{
    public class Form1Service : InterviewService, IForm1Service
    {
        public Form1Service(IRemoteUnitOfWork uow, IMapper mapper) : base(uow, mapper) { }

        public IEnumerable<InterviewDTO> GetF1R2Interviews(string questionnaireId, int offset, int limit, string region = null)
        {
            return CollectInterviews(
                Uow.Form1Repository.GetF1R2Interviews(
                    questionnaireId, offset, limit, region
                )
            );
        }

        public async Task<string> GetInterviewDate(string interviewId)
        {
            return await Uow.Form1Repository.GetInterviewDate(interviewId);
        }

        public async Task<string> GetMemberBirthDate(string interviewId, string section)
        {
            return await Uow.Form1Repository.GetMemberBirthDate(interviewId, section);
        }

        public async Task<string> GetMemberMartialStatus(string interviewId, string section)
        {
            return await Uow.Form1Repository.GetMemberMaritalStatus(interviewId, section);
        }

        public async Task<bool> HasMemberSpouse(string interviewId)
        {
            return await Uow.Form1Repository.HasMemberSpouse(interviewId);
        }
    }
}
