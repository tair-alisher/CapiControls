using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces
{
    public interface IPaginated<T> where T : class
    {
        IEnumerable<T> GetAll(int pageSize, int page);
        int CountAll();
    }
}
