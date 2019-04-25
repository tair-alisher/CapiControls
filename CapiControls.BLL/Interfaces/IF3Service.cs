using CapiControls.BLL.DTO;
using System.Collections.Generic;

namespace CapiControls.BLL.Interfaces
{
    public interface IF3Service
    {
        IEnumerable<InterviewDTO> GetF3R1UnitsInterviews(string questionnaireId, int offset, int limit, string region = null);
        IEnumerable<InterviewDTO> getF3R2UnitsInterviews(string questionnaireId, int offset, int limit, string region = null);
    }
}
