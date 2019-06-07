using System.Threading.Tasks;

namespace CapiControls.Controls.Interfaces
{
    public interface IControlAsync
    {
        Task<string> Execute(string questionnaireId, string region = null);
    }
}
