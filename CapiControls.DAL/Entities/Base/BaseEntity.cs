using CapiControls.DAL.Interfaces;

namespace CapiControls.DAL.Entities.Base
{
    public class BaseEntity : IBaseEntity { }

    public class BaseEntity<T> : IBaseEntity<T>
    {
        public T Id { get; set; }
    }
}
