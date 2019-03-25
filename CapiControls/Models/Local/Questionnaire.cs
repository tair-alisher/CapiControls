using System;
using System.ComponentModel.DataAnnotations;

namespace CapiControls.Models.Local
{
    public class Questionnaire
    {
        [Key]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string Group { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }
    }
}
