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

        public List<Interview> GetF3R1UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit)
        {
            string query = base.BaseQuery + @"
                        and question_entity.stata_export_caption = 'f3r1q6'
                    order by interview_id
                    offset @_offset
                    limit @_limit
                ";

            IEnumerable<RawInterviewData> rawData = Enumerable.Empty<RawInterviewData>();
            using (var connection = Connection)
            {
                rawData = connection.Query<RawInterviewData>(query, new
                {
                    no_answer = "",
                    questionnaire_id = questionnaireId,
                    _offset = offset,
                    _limit = limit
                });
            }

            return base.CollectInterviews(rawData);
        }

        public List<Interview> GetF3R2UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit)
        {
            string query = base.BaseQuery + @"
                    and (question_entity.stata_export_caption like 'f3r2q5b%'
                        or question_entity.stata_export_caption = 'f3r2nenaideno')
                    order by interview_id
                    offset @offset
                    limit @limit
                ";

            IEnumerable<RawInterviewData> rawData = Enumerable.Empty<RawInterviewData>();
            using (var connection = Connection)
            {
                rawData = connection.Query<RawInterviewData>(query, new
                {
                    no_answer = "",
                    questionnaire_id = questionnaireId,
                    offset,
                    limit
                });
            }

            return base.CollectInterviews(rawData);
        }
    }
}
