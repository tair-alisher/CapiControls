using CapiControls.BLL.DTO.Base;

namespace CapiControls.BLL.DTO
{
    public class QuestionDataDTO : BaseDTO
    {
        public string QuestionSection { get; set; }
        public string QuestionCode { get; set; }
        public string Answer { get; set; }
    }
}
