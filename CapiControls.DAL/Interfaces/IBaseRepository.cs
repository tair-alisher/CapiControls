using System;
using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        void Add(T item);
        T Find(Guid id);
        IEnumerable<T> GetAll();
        void Update(T item);
        void Delete(Guid id);
        void Delete(T item);
    }
}
