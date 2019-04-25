using CapiControls.BLL.DTO;
using System;
using System.Collections.Generic;

namespace CapiControls.BLL.Interfaces
{
    public interface IQuestionnaireService : IBaseService
    {
        IEnumerable<QuestionnaireDTO> GetQuestionnaires();
        IEnumerable<QuestionnaireDTO> GetQuestionnaires(int page, int pageSize);
        IEnumerable<QuestionnaireDTO> GetQuestionnairesByGroupName(string group);
        QuestionnaireDTO GetQuestionnaire(Guid id);
        int CountQuestionnaires();
        void AddQuestionnaire(QuestionnaireDTO questionnaire);
        void UpdateQuestionnaire(QuestionnaireDTO questionnaire);
        void DeleteQuestionnaire(Guid id);
    }
}
