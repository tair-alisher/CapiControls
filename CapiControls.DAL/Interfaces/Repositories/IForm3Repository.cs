using CapiControls.DAL.Common;
using CapiControls.DAL.Entities;
using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces.Repositories
{
    public interface IForm3Repository
    {
        List<F3AnswerData> GetF3R1InterviewsData(QueryParams parameters);
        List<F3AnswerData> GetF3R2InterviewsData(QueryParams parameters);
    }
}
