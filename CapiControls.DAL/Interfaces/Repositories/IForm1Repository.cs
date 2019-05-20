using CapiControls.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapiControls.DAL.Interfaces.Repositories
{
    public interface IForm1Repository
    {
        IEnumerable<RawInterviewData> GetF1R2Interviews(string questionnaireId, int offset, int limit, string region = null);
        Task<string> GetInterviewDate(string interviewId);
        Task<string> GetMemberBirthDate(string interviewId, string section);
        Task<string> GetMemberMaritalStatus(string interviewId, string section);
        Task<bool> HasMemberSpouse(string interviewId);
        Task<bool> IsMemberAbsenceReasonAnswered(string interviewId, string section);
    }
}
