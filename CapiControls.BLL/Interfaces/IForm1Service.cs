using CapiControls.BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapiControls.BLL.Interfaces
{
    public interface IForm1Service
    {
        IEnumerable<InterviewDTO> GetF1R2Interviews(string questionnaireId, int offset, int limit, string region = null);
        Task<string> GetInterviewDate(string interviewId);
        Task<string> GetMemberBirthDate(string interviewId, string section);
        Task<string> GetMemberMartialStatus(string interviewId, string section);
        Task<bool> HasMemberSpouse(string interviewId);
    }
}
