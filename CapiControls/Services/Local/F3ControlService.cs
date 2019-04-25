using CapiControls.Controls.Interfaces;
using CapiControls.Services.Interfaces;

namespace CapiControls.Services.Local
{
    public class F3ControlService : IF3ControlService
    {
        private readonly IF3R1UnitsControl F3R1UnitsControl;
        private readonly IF3R2UnitsControl F3R2UnitsControl;

        public F3ControlService(IF3R1UnitsControl f3r1unitsControl, IF3R2UnitsControl f3r2unitsControl)
        {
            F3R1UnitsControl = f3r1unitsControl;
            F3R2UnitsControl = f3r2unitsControl;
        }

        public string ExecuteF3R1UnitsControl(string questionnaireId, string region = null)
        {
            return F3R1UnitsControl.Execute(questionnaireId, region);
        }

        public string ExecuteF3R2UnitsControl(string questionnaireId, string region = null)
        {
            return F3R2UnitsControl.Execute(questionnaireId, region);
        }
    }
}
