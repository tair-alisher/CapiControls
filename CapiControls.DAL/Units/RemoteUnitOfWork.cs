using CapiControls.DAL.Interfaces.Repositories;
using CapiControls.DAL.Interfaces.Units;
using CapiControls.DAL.Repositories.Remote;

namespace CapiControls.DAL.Units
{
    public class RemoteUnitOfWork : UnitOfWork, IRemoteUnitOfWork
    {
        private IInterviewRepository _interviewRepository;
        private IForm3Repository _form3Repository;
        private IForm1Repository _form1Repository;

        public RemoteUnitOfWork(string connectionString) : base(connectionString) { }

        public IInterviewRepository InterviewRepository
        {
            get
            {
                return _interviewRepository ?? (_interviewRepository = new InterviewRepository(Transaction));
            }
        }

        public IForm3Repository Form3Repository
        {
            get
            {
                return _form3Repository ?? (_form3Repository = new Form3Repository(Transaction));
            }
        }

        public IForm1Repository Form1Repository
        {
            get
            {
                return _form1Repository ?? (_form1Repository = new Form1Repository(Transaction));
            }
        }

        public override void ResetRepositories()
        {
            _interviewRepository = null;
            _form3Repository = null;
        }
    }
}
