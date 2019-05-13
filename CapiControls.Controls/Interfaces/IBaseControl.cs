using CapiControls.BLL.DTO;
using System.Collections.Generic;

namespace CapiControls.Controls.Interfaces
{
    public interface IBaseControl
    {
        IEnumerable<QuestionnaireDTO> GetQuestionnairesByGroupName(string group);
        IEnumerable<RegionDTO> GetRegions();
    }
}
