using CapiControls.Models.Server;
using System.Collections.Generic;

namespace CapiControls.Data.Interfaces
{
    public interface IForm3Repository
    {
        List<Interview> GetF3R1UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null);
        List<Interview> GetF3R2UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null);
    }
}
