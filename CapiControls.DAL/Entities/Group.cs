using CapiControls.DAL.Entities.Base;
using System;

namespace CapiControls.DAL.Entities
{
    public class Group : BaseEntity<Guid>
    {
        public string Title { get; set; }
    }
}
