using CapiControls.Models.Local;
using System.Collections.Generic;

namespace CapiControls.Services.Interfaces
{
    public interface IControlService
    {
        IEnumerable<Questionnaire> GetQuestionnairesByGroupName(string group);
        void ExecuteF3R1UnitsControl(string questionnaireId);
    }
}
