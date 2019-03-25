using System.Collections.Generic;

namespace CapiControls.Data.Interfaces
{
    public interface IPaginated<T> where T : class
    {
        IEnumerable<T> GetAll(int pageSize, int page);
        int CountAll();
    }
}
