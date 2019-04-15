using CapiControls.Models.Local;
using System.Collections.Generic;

namespace CapiControls.Services.Interfaces
{
    public interface IBaseControlService
    {
        IEnumerable<Questionnaire> GetQuestionnairesByGroupName(string group);
        IEnumerable<Region> GetRegions();
    }
}
