using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Units;
using System;
using System.Collections.Generic;

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
            _uow.QuestionnaireRepository.Add(Mapper.Map<QuestionnaireDTO, Questionnaire>(questionnaire));
            _uow.Commit();
        }

        public int CountQuestionnaires()
        {
            return _uow.QuestionnaireRepository.CountAll();
        }

        public void DeleteQuestionnaire(Guid id)
        {
            _uow.QuestionnaireRepository.Delete(id);
            _uow.Commit();
        }

        public QuestionnaireDTO GetQuestionnaire(Guid id)
        {
            return Mapper.Map<Questionnaire, QuestionnaireDTO>(_uow.QuestionnaireRepository.Find(id));
        }

        public IEnumerable<QuestionnaireDTO> GetQuestionnaires(int page, int pageSize)
        {
            return Mapper.Map<IEnumerable<Questionnaire>, IEnumerable<QuestionnaireDTO>>(_uow.QuestionnaireRepository.GetAll(page, pageSize));
        }

        public void UpdateQuestionnaire(QuestionnaireDTO questionnaire)
        {
            _uow.QuestionnaireRepository.Update(Mapper.Map<QuestionnaireDTO, Questionnaire>(questionnaire));
            _uow.Commit();
        }
    }
}
