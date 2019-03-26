using System.Collections.Generic;
using System.Linq;
using CapiControls.Controls.Interfaces;
using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using CapiControls.Services.Interfaces;

namespace CapiControls.Services.Local
{
    public class ControlService : IControlService
    {
        private readonly IQuestionnaireRepository QuestRepository;
        private readonly IF3R1UnitsControl F3R1UnitsControl;

        public ControlService(IQuestionnaireRepository questRepository, IF3R1UnitsControl f3r1unitsControl)
        {
            QuestRepository = questRepository;
            F3R1UnitsControl = f3r1unitsControl;
        }

        public IEnumerable<Questionnaire> GetQuestionnairesByGroupName(string group)
        {
            return QuestRepository.GetAll().Where(q => q.Group == group).ToList();
        }

        public void ExecuteF3R1UnitsControl(string questionnaireId)
        {
            F3R1UnitsControl.Execute(questionnaireId);
        }
    }
}
