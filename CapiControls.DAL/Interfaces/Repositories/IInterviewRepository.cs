using CapiControls.DAL.Entities;
using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces.Repositories
{
    public interface IInterviewRepository
    {
        IEnumerable<RawInterviewData> GetInterviewsDataByQuestionnaire(string questionnaireId, int offset, int limit);
        IEnumerable<RawInterviewData> GetInterviewsDataByQuestionnaireAndRegion(string questionnaireId, string region, int offset, int limit);
        IEnumerable<RawInterviewData> GetInterviewsDataByQuestionnaireAndQuestionCode(string questionnaireId, string questionCode, int offset, int limit);
        string GetQuestionAnswerBySection(string interviewId, string questionCode, string section);
        string GetQuestionFirstAnswer(string interviewId, string questionCode);
        string GetInterviewKey(string interviewId);
    }
}
