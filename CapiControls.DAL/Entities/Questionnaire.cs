using CapiControls.DAL.Entities.Base;
using System;

namespace CapiControls.DAL.Entities
{
    public class Questionnaire : BaseEntity<Guid>
    {
        public Guid GroupId { get; set; }
        public string Group { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }
    }
}
