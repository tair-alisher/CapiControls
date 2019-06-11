using CapiControls.DAL.Interfaces.Repositories;

namespace CapiControls.DAL.Interfaces.Units
{
    public interface IRemoteUnitOfWork : IUnitOfWork
    {
        IInterviewRepository InterviewRepository { get; }
        IForm1Repository Form1Repository { get; }
        IForm3Repository Form3Repository { get; }
        IForm5Repository Form5Repository { get; }
    }
}
