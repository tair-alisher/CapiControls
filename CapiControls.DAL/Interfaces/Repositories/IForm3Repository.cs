using CapiControls.DAL.Common;
using CapiControls.DAL.Entities;
using System.Collections.Generic;

namespace CapiControls.DAL.Interfaces.Repositories
{
    public interface IForm3Repository
    {
        IEnumerable<RawInterviewData> GetF3R1UnitsInterviewsData(QueryParams parameters);
        IEnumerable<RawInterviewData> GetF3R2UnitsInterviewsData(QueryParams parameters);
        IEnumerable<RawInterviewData> GetF3R2SupplySourcesInterviewsData(QueryParams parameters);
    }
}
