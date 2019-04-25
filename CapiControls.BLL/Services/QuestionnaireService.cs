using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.BLL.Services
{
    public class QuestionnaireService : BaseService, IQuestionnaireService
    {
        private readonly ILocalUnitOfWork _uow;

        public QuestionnaireService(ILocalUnitOfWork uow, IMapper mapper) : base(mapper)
        {
            _uow = uow;
        }

        public void AddQuestionnaire(QuestionnaireDTO questionnaire)
        {
            questionnaire.Id = Guid.NewGuid();
            _uow.QuestionnaireRepository.Add(Mapper.Map<QuestionnaireDTO, Questionnaire>(questionnaire));
            _uow.Commit();
        }

        public int CountQuestionnaires()
        {
            return _uow.QuestionnaireRepository.CountAll();
        }

        public QuestionnaireDTO GetQuestionnaire(Guid id)
        {
            return Mapper.Map<Questionnaire, QuestionnaireDTO>(_uow.QuestionnaireRepository.Find(id));
        }

        public IEnumerable<QuestionnaireDTO> GetQuestionnaires()
        {
            return Mapper.Map<IEnumerable<Questionnaire>, IEnumerable<QuestionnaireDTO>>(
                _uow.QuestionnaireRepository.GetAll()
            );
        }

        public IEnumerable<QuestionnaireDTO> GetQuestionnaires(int page, int pageSize)
        {
            return Mapper.Map<IEnumerable<Questionnaire>, IEnumerable<QuestionnaireDTO>>(_uow.QuestionnaireRepository.GetAll(page, pageSize));
        }

        public IEnumerable<QuestionnaireDTO> GetQuestionnairesByGroupName(string group)
        {
            var questionnaires = _uow.QuestionnaireRepository.GetAll().Where(q => q.Group == group).ToList();
            return Mapper.Map<IEnumerable<Questionnaire>, IEnumerable<QuestionnaireDTO>>(questionnaires);
        }

        public void UpdateQuestionnaire(QuestionnaireDTO questionnaire)
        {
            _uow.QuestionnaireRepository.Update(Mapper.Map<QuestionnaireDTO, Questionnaire>(questionnaire));
            _uow.Commit();
        }

        public void DeleteQuestionnaire(Guid id)
        {
            _uow.QuestionnaireRepository.Delete(id);
            _uow.Commit();
        }
    }
}
