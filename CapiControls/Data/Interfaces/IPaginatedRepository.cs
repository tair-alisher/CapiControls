namespace CapiControls.Data.Interfaces
{
    public interface IPaginatedRepository<T> : IRepository<T>, IPaginated<T> where T : class
    {
    }
}
