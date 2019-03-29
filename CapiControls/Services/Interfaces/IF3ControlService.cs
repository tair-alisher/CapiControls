using CapiControls.Models.Local;
using System.Collections.Generic;

namespace CapiControls.Services.Interfaces
{
    public interface IF3ControlService
    {
        IEnumerable<Questionnaire> GetQuestionnairesByGroupName(string group);
        string ExecuteF3R1UnitsControl(string questionnaireId);
        string ExecuteF3R2UnitsControl(string questionnaireId);
    }
}
