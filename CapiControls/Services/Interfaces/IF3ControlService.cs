namespace CapiControls.Services.Interfaces
{
    public interface IF3ControlService
    {
        string ExecuteF3R1UnitsControl(string questionnaireId, string region = null);
        string ExecuteF3R2UnitsControl(string questionnaireId, string region = null);
    }
}
