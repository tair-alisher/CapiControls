using System;
using System.ComponentModel.DataAnnotations;

namespace CapiControls.Models.Local
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Наименование")]
        public string Title { get; set; }
    }
}
