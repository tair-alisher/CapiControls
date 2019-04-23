using CapiControls.BLL.DTO.Base;
using System;

namespace CapiControls.BLL.DTO
{
    public class QuestionnaireDTO : BaseDTO<Guid>
    {
        public Guid GroupId { get; set; }
        public string Group { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }r
    }
}
