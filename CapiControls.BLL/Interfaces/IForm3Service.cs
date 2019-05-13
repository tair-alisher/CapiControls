using CapiControls.BLL.DTO;
using System.Collections.Generic;

namespace CapiControls.BLL.Interfaces
{
    public interface IForm3Service
    {
        IEnumerable<InterviewDTO> GetF3R1UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null);
        IEnumerable<InterviewDTO> GetF3R2UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null);
    }
}
