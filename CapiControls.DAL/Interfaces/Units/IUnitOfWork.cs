using System;

namespace CapiControls.DAL.Interfaces.Units
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}
