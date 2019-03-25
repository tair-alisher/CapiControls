using System;
using System.Collections.Generic;

namespace CapiControls.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Create(T item);
        T Get(Guid id);
        IEnumerable<T> GetAll(int itemsPerPage, int page);
        void Update(T item);
        void Delete(Guid id);
        int CountAll();
    }
}
