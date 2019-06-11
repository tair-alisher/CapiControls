using CapiControls.Controls.Interfaces.Form3;

namespace CapiControls.Controls.Controls.Form3
{
    public class F3Controls : IF3Controls
    {
        private readonly IF3R1UnitsControl _f3R1UnitsControl;
        private readonly IF3R2UnitsControl _f3R2UnitsControl;
        private readonly IF3R2SupplySourcesControl _f3R2SupplySourcesControl;

        public F3Controls(
            IF3R1UnitsControl f3R1UnitsContorl,
            IF3R2UnitsControl f3R2UnitsControl,
            IF3R2SupplySourcesControl f3R2SupplySourcesControl)
        {
            _f3R1UnitsControl = f3R1UnitsContorl;
            _f3R2UnitsControl = f3R2UnitsControl;
            _f3R2SupplySourcesControl = f3R2SupplySourcesControl;
        }

        public string ExecuteF3R1UnitsControl(string questionnaireId, string region = null)
        {
            return _f3R1UnitsControl.Execute(questionnaireId, region);
        }

        public string ExecuteF3R2UnitsControl(string questionnaireId, string region = null)
        {
            return _f3R2UnitsControl.Execute(questionnaireId, region);
        }

        public string ExecuteF3R2SupplySources(string questionnaireId, string region = null)
        {
            return _f3R2SupplySourcesControl.Execute(questionnaireId, region);
        }
    }
}
