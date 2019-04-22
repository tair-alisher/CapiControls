using CapiControls.DAL.Interfaces.Repositories;

namespace CapiControls.DAL.Interfaces.Units
{
    public interface IRemoteUnitOfWork : IUnitOfWork
    {
        IInterviewRepository InterviewRepository { get; }
        IForm3Repository Form3Repository { get; }
    }
}
