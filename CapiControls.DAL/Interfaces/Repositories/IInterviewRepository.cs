using CapiControls.DAL.Entities;
using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces.Repositories
{
    public interface IInterviewRepository
    {
        List<Interview> GetInterviewsByQuestionnaire(string questionnaireId, int offset, int limit);
        List<Interview> GetInterviewsByQuestionnaireAndRegion(string questionnaireId, string region, int offset, int limit);
        List<Interview> GetInterviewsByQuestionnaireAndQuestionCode(string questionnaireId, string questionCode, int offset, int limit);
        List<Interview> CollectInterviews(IEnumerable<RawInterviewData> rawData);
        string GetQuestionAnswerBySection(string interviewId, string questionCode, string section);
        string GetQuestionFirstAnswer(string interviewId, string questionCode);
        string GetInterviewKey(string interviewId);
    }
}
