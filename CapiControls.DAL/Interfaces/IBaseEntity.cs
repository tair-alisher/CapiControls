namespace CapiControls.DAL.Interfaces
{
    public interface IBaseEntity { }

    public interface IBaseEntity<T> : IBaseEntity
    {
        T Id { get; set; }
    }
}
