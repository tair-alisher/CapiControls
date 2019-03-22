using CapiControls.Models.Server;
using System.Collections.Generic;

namespace CapiControls.Data.Interfaces
{
    public interface IInterviewRepository
    {
        List<Interview> GetInterviewsByQuestionnaire(string questionnaireId, int offset, int limit);
    }
}
