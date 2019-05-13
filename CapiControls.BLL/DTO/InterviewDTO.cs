using CapiControls.BLL.DTO.Base;
using System.Collections.Generic;

namespace CapiControls.BLL.DTO
{
    public class InterviewDTO : BaseDTO<string>
    {
        public string QuestionnaireId { get; set; }
        public List<QuestionDataDTO> QuestionData { get; set; }
    }
}
