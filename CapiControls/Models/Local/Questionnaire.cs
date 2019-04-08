using System;
using System.ComponentModel.DataAnnotations;

namespace CapiControls.Models.Local
{
    public class Questionnaire
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Группа")]
        public Guid GroupId { get; set; }
        [Display(Name = "Группа")]
        public string Group { get; set; }
        [Display(Name = "Идентификатор")]
        public string Identifier { get; set; }
        [Display(Name = "Наименование")]
        public string Title { get; set; }
    }
}
