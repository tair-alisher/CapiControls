using CapiControls.DAL.Entities;
using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces.Repositories
{
    public interface IForm3Repository
    {
        IEnumerable<RawInterviewData> GetF3R1UnitsInterviewsDataByQuestionnaire(string questionnaireId, int offset, int limit, string region = null);
        IEnumerable<RawInterviewData> GetF3R2UnitsInterviewsDataByQuestionnaire(string questionnaireId, int offset, int limit, string region = null);
    }
}
