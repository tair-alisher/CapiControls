using CapiControls.Data.Interfaces;
using CapiControls.Models.Server;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Data.Repositories.Server
{
    public class Form3Repository : InterviewRepository, IForm3Repository
    {
        public Form3Repository(IConfiguration configuration) : base(configuration) { }

        public List<Interview> GetF3R1UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null)
        {
            string query = base.DefaultSelect
                + base.DefaultFrom
                + (region == null ? base.DefaultWhere : base.RegionWhere)
                + "and question_entity.stata_export_caption = 'f3r1q6'\n"
                + base.DefaultOffsetLimit;

            IEnumerable<RawInterviewData> rawData = Enumerable.Empty<RawInterviewData>();
            using (var connection = Connection)
            {
                rawData = connection.Query<RawInterviewData>(query, new
                {
                    questionnaireId,
                    offset,
                    region,
                    limit
                });
            }

            return base.CollectInterviews(rawData);
        }

        public List<Interview> GetF3R2UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null)
        {
            string query = base.DefaultSelect
                + base.DefaultFrom
                + (region == null ? base.DefaultWhere : base.RegionWhere)
                + "and (question_entity.stata_export_caption like 'f3r2q5b%' or question_entity.stata_export_caption = 'f3r2nenaideno')\n"
                + base.DefaultOffsetLimit;

            IEnumerable<RawInterviewData> rawData = Enumerable.Empty<RawInterviewData>();
            using (var connection = Connection)
            {
                rawData = connection.Query<RawInterviewData>(query, new
                {
                    questionnaireId,
                    offset,
                    region,
                    limit
                });
            }

            return base.CollectInterviews(rawData);
        }
    }
}
