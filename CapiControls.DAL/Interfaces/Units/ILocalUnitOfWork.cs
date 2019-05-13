using CapiControls.DAL.Interfaces.Repositories;

namespace CapiControls.DAL.Interfaces.Units
{
    public interface ILocalUnitOfWork : IUnitOfWork
    {
        IGroupRepository GroupRepository { get; }
        IQuestionnaireRepository QuestionnaireRepository { get; }
        IRegionRepository RegionRepository { get; }
        IUserRepository UserRepository { get; }
    }
}
