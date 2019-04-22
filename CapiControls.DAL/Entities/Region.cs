using CapiControls.DAL.Entities.Base;
using System;

namespace CapiControls.DAL.Entities
{
    public class Region : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
