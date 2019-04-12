using System;
using System.ComponentModel.DataAnnotations;

namespace CapiControls.Models.Local
{
    public class Region
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Код области")]
        public string Name { get; set; }
        [Display(Name = "Область")]
        public string Title { get; set; }

    }
}
