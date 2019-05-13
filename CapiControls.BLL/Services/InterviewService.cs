using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Units;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.BLL.Services
{
    public class InterviewService : BaseService, IInterviewService
    {
        protected readonly IRemoteUnitOfWork Uow;

        public InterviewService(IRemoteUnitOfWork uow, IMapper mapper) : base(mapper)
        {
            Uow = uow;
        }

        public string GetInterviewKey(string interviewId)
        {
            return Uow.InterviewRepository.GetInterviewKey(interviewId);
        }

        public string GetQuestionAnswerBySection(string interviewId, string questionCode, string section)
        {
            return Uow.InterviewRepository.GetQuestionAnswerBySection(interviewId, questionCode, section);
        }

        public string GetQuestionFirstAnswer(string interviewId, string questionCode)
        {
            return Uow.InterviewRepository.GetQuestionFirstAnswer(interviewId, questionCode);
        }

        public IEnumerable<InterviewDTO> GetInterviewsByQuestionnaire(string questionnaireId, int offset, int limit)
        {
            return CollectInterviews(
                Uow.InterviewRepository.GetInterviewsDataByQuestionnaire(questionnaireId, offset, limit)
            );
        }

        public IEnumerable<InterviewDTO> GetInterviewsByQuestionnaireAndRegion(string questionnaireId, string region, int offset, int limit)
        {
            return CollectInterviews(
                Uow.InterviewRepository.GetInterviewsDataByQuestionnaireAndRegion(questionnaireId, region, offset, limit)
            );
        }

        public IEnumerable<InterviewDTO> GetInterviewsByQuestionnaireAndQuestionCode(string questionnaireId, string questionCode, int offset, int limit)
        {
            return CollectInterviews(
                Uow.InterviewRepository.GetInterviewsDataByQuestionnaireAndQuestionCode(
                    questionnaireId, questionCode, offset, limit
                )
            );
        }

        protected List<InterviewDTO> CollectInterviews(IEnumerable<RawInterviewData> rawData)
        {
            var interviews = new List<InterviewDTO>();
            InterviewDTO interview = null;
            QuestionDataDTO questionData = null;
            foreach (var row in rawData)
            {
                bool interviewAlreadyAdded = interviews.Where(i => i.Id == row.InterviewId).Count() > 0;

                if (interviewAlreadyAdded)
                {
                    interview = interviews.Where(i => i.Id == row.InterviewId).First();
                    questionData = new QuestionDataDTO
                    {
                        QuestionSection = row.QuestionSectionSuffix.Length > 0 ?
                            $"{row.QuestionSection}_{row.QuestionSectionSuffix}" :
                            row.QuestionSection,
                        QuestionCode = row.QuestionCode,
                        Answer = row.Answer
                    };
                    interview.QuestionData.Add(questionData);
                }
                else
                {
                    interview = new InterviewDTO
                    {
                        Id = row.InterviewId,
                        QuestionnaireId = row.QuestionnaireId,
                        QuestionData = new List<QuestionDataDTO>()
                    };
                    questionData = new QuestionDataDTO
                    {
                        QuestionSection = row.QuestionSectionSuffix.Length > 0 ?
                            $"{row.QuestionSection}_{row.QuestionSectionSuffix}" :
                            row.QuestionSection,
                        QuestionCode = row.QuestionCode,
                        Answer = row.Answer
                    };
                    interview.QuestionData.Add(questionData);

                    interviews.Add(interview);
                }
            }

            return interviews;
        }
    }
}
