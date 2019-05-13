namespace CapiControls.DAL.Interfaces
{
    public interface IPaginatedRepository<T> : IBaseRepository<T>, IPaginated<T> where T : class
    {
    }
}
