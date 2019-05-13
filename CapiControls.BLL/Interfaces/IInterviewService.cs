using CapiControls.BLL.DTO;
using System.Collections.Generic;

namespace CapiControls.BLL.Interfaces
{
    public interface IInterviewService
    {
        string GetQuestionAnswerBySection(string interviewId, string questionCode, string section);
        string GetQuestionFirstAnswer(string interviewId, string questionCode);
        string GetInterviewKey(string interviewId);

        IEnumerable<InterviewDTO> GetInterviewsByQuestionnaire(string questionnaireId, int offset, int limit);
        IEnumerable<InterviewDTO> GetInterviewsByQuestionnaireAndRegion(string questionnaire, string region, int offset, int limit);
        IEnumerable<InterviewDTO> GetInterviewsByQuestionnaireAndQuestionCode(string questionnaireId, string questionCode, int offset, int limit);
    }
}
