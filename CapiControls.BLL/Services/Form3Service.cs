using AutoMapper;
using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Interfaces.Units;
using System.Collections.Generic;

namespace CapiControls.BLL.Services
{
    public class Form3Service : InterviewService, IForm3Service
    {
        public Form3Service(IRemoteUnitOfWork uow, IMapper mapper) : base(uow, mapper) { }

        public IEnumerable<InterviewDTO> GetF3R1UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null)
        {
            return CollectInterviews(
                Uow.Form3Repository.GetF3R1UnitsInterviewsDataByQuestionnaire(
                    questionnaireId, offset, limit, region
                )
            );
        }

        public IEnumerable<InterviewDTO> GetF3R2UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null)
        {
            return CollectInterviews(
                Uow.Form3Repository.GetF3R2UnitsInterviewsDataByQuestionnaire(
                    questionnaireId, offset, limit, region
                )
            );
        }
    }
}
