using CapiControls.Models.Local;

namespace CapiControls.Data.Interfaces
{
    public interface IGroupRepository : IRepository<Group>, IPaginated<Group>
    {
    }
}
