using CapiControls.DAL.Entities.Base;
using System;

namespace CapiControls.DAL.Entities.Account
{
    public class Role : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
