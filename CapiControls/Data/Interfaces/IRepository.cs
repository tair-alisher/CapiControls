using System;
using System.Collections.Generic;

namespace CapiControls.Data.Interfaces
{
    public interface IInterviewRepository<T> where T : class
    {
        void Create(T item);
        T Get(Guid id);
        IEnumerable<T> GetAll();
        void Update(T item);
        void Delete(Guid id);
    }
}
