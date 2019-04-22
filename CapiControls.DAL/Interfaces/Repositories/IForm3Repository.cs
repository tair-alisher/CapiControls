using CapiControls.DAL.Entities;
using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces.Repositories
{
    public interface IForm3Repository
    {
        List<Interview> GetF3R1UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null);
        List<Interview> GetF3R2UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null);
    }
}
