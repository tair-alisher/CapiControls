using CapiControls.Controls.Interfaces;
using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using CapiControls.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Services.Local
{
    public class F3ControlService : IF3ControlService
    {
        private readonly IPaginatedRepository<Questionnaire> QuestRepository;
        private readonly IF3R1UnitsControl F3R1UnitsControl;
        private readonly IF3R2UnitsControl F3R2UnitsControl;

        public F3ControlService(IPaginatedRepository<Questionnaire> questRepository, IF3R1UnitsControl f3r1unitsControl, IF3R2UnitsControl f3r2unitsControl)
        {
            QuestRepository = questRepository;
            F3R1UnitsControl = f3r1unitsControl;
            F3R2UnitsControl = f3r2unitsControl;
        }

        public IEnumerable<Questionnaire> GetQuestionnairesByGroupName(string group)
        {
            return QuestRepository.GetAll().Where(q => q.Group == group).ToList();
        }

        public string ExecuteF3R1UnitsControl(string questionnaireId)
        {
            return F3R1UnitsControl.Execute(questionnaireId);
        }

        public string ExecuteF3R2UnitsControl(string questionnaireId)
        {
            return F3R2UnitsControl.Execute(questionnaireId);
        }
    }
}
