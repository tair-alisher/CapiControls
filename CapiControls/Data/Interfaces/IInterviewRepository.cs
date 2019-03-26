using CapiControls.Models.Server;
using System.Collections.Generic;

namespace CapiControls.Data.Interfaces
{
    public interface IInterviewRepository
    {
        List<Interview> GetInterviewsByQuestionnaire(string questionnaireId, int offset, int limit);
        List<Interview> GetInterviewsByQuestionnaireAndQuestionCode(string questionnaireId, string questionCode, int offset, int limit);
        List<Interview> GetF1R3InterviewsByQuestionnaire(string questionnaireId, int offset, int limit);
        string GetQuestionAnswer(string interviewId, string questionCode);
    }
}
