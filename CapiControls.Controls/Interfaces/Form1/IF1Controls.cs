using System.Threading.Tasks;

namespace CapiControls.Controls.Interfaces.Form1
{
    public interface IF1Controls
    {
        Task<string> ExecuteF1R2HhMembersControl(string questionnaireId, string region = null);
    }
}
