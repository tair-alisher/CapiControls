using CapiControls.DAL.Interfaces.Repositories;
using CapiControls.DAL.Interfaces.Units;
using CapiControls.DAL.Repositories;

namespace CapiControls.DAL.Units
{
    public class LocalUnitOfWork : UnitOfWork, ILocalUnitOfWork
    {
        private IGroupRepository _groupRepository;
        private IQuestionnaireRepository _questionnaireRepository;
        private IRegionRepository _regionRepository;
        private IUserRepository _userRepository;

        public LocalUnitOfWork(string connectionString) : base(connectionString) { }

        public IGroupRepository GroupRepository
        {
            get
            {
                return _groupRepository ?? (_groupRepository = new GroupRepository(Transaction));
            }
        }

        public IQuestionnaireRepository QuestionnaireRepository
        {
            get
            {
                return _questionnaireRepository ?? (_questionnaireRepository = new QuestionnaireRepository(Transaction));
            }
        }

        public IRegionRepository RegionRepository
        {
            get
            {
                return _regionRepository ?? (_regionRepository = new RegionRepository(Transaction));
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository ?? (_userRepository = new UserRepository(Transaction));
            }
        }

        public override void ResetRepositories()
        {
            _groupRepository = null;
            _questionnaireRepository = null;
            _regionRepository = null;
            _userRepository = null;
        }
    }
}
