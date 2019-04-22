using System.Data;

namespace CapiControls.DAL.Repositories.Base
{
    internal abstract class BaseRepository
    {
        protected IDbTransaction Transaction { get; private set; }
        protected IDbConnection Connection
        {
            get
            {
                return Transaction.Connection;
            }
        }

        public BaseRepository(IDbTransaction transaction)
        {
            Transaction = transaction;
        }
    }
}
