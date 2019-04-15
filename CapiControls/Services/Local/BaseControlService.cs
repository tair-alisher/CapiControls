using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using CapiControls.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Services.Local
{
    public class BaseControlService : IBaseControlService
    {
        protected readonly IPaginatedRepository<Questionnaire> QuestRepository;
        protected readonly IRepository<Region> RegionRepository;

        public BaseControlService(IPaginatedRepository<Questionnaire> questRepository, IRepository<Region> regionRepository)
        {
            QuestRepository = questRepository;
            RegionRepository = regionRepository;
        }

        public IEnumerable<Questionnaire> GetQuestionnairesByGroupName(string group)
        {
            return QuestRepository.GetAll().Where(q => q.Group == group).ToList();
        }

        public IEnumerable<Region> GetRegions()
        {
            return RegionRepository.GetAll().ToList();
        }
    }
}
