namespace CapiControls.Controls.Interfaces.Form3
{
    public interface IF3Controls
    {
        string ExecuteF3R1UnitsControl(string questionnaireId, string region = null);
        string ExecuteF3R2UnitsControl(string questionnaireId, string region = null);
        string ExecuteF3R2SupplySources(string questionnaireId, string region = null);
    }
}
