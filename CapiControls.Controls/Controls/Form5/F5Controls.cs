using CapiControls.Controls.Interfaces.Form5;

namespace CapiControls.Controls.Controls.Form5
{
    public class F5Controls : IF5Controls
    {
        private readonly IF5Control _f5Control;

        public F5Controls(IF5Control f5Control)
        {
            _f5Control = f5Control;
        }

        public string ExecuteF5Controls(string questionnaireId, string region = null)
        {
            return _f5Control.Execute(questionnaireId, region);
        }
    }
}
