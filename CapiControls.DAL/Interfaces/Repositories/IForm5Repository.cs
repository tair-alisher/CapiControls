using CapiControls.DAL.Common;
using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces.Repositories
{
    public interface IForm5Repository
    {
        List<F5AnswerData> GetF5InterviewsData(QueryParams parameters);
    }
}
