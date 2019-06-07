using CapiControls.Controls.Interfaces.Form1;
using System.Threading.Tasks;

namespace CapiControls.Controls.Controls.Form1
{
    public class F1Controls : IF1Controls
    {
        private readonly IF1R2HhMembersControl _f1R2HhMembersControl;

        public F1Controls(IF1R2HhMembersControl f1R2HhMembersControl)
        {
            _f1R2HhMembersControl = f1R2HhMembersControl;
        }

        public async Task<string> ExecuteF1R2HhMembersControl(string questionnaireId, string region = null)
        {
            return await _f1R2HhMembersControl.Execute(questionnaireId, region);
        }
    }
}
