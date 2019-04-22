using CapiControls.DAL.Interfaces.Units;
using Npgsql;
using System;
using System.Data;

namespace CapiControls.DAL.Units
{
    public class UnitOfWork : IUnitOfWork
    {
        protected IDbConnection Connection;
        protected IDbTransaction Transaction;
        private bool _disposed;

        public UnitOfWork(string connectionString)
        {
            Connection = new NpgsqlConnection(connectionString);
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                Transaction.Commit();
            }
            catch
            {
                Transaction.Rollback();
                throw;
            }
            finally
            {
                Transaction.Dispose();
                Transaction = Connection.BeginTransaction();
                ResetRepositories();
            }
        }

        public virtual void ResetRepositories() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (Transaction != null)
                    {
                        Transaction.Dispose();
                        Transaction = null;
                    }
                    if (Connection != null)
                    {
                        Connection.Dispose();
                        Connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
