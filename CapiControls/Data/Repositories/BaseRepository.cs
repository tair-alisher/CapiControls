using System.Data;

namespace CapiControls.Data.Repositories
{
    public abstract class BaseRepository
    {
        public abstract IDbConnection Connection
        {
            get;
        }
    }
}
