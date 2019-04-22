using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Repositories;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CapiControls.DAL.Repositories.Remote
{
    internal class Form3Repository : InterviewRepository, IForm3Repository
    {
        public Form3Repository(IDbTransaction transaction) : base(transaction) { }

        public List<Interview> GetF3R1UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null)
        {
            string query = base.DefaultSelect
                + base.DefaultFrom
                + (region == null ? base.DefaultWhere : base.RegionWhere)
                + "and question_entity.stata_export_caption = 'f3r1q6'\n"
                + base.DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new { questionnaireId, region, offset, limit },
                transaction: Transaction
            ) ?? Enumerable.Empty<RawInterviewData>();

            return CollectInterviews(rawData);
        }

        public List<Interview> GetF3R2UnitsInterviewsByQuestionnaire(string questionnaireId, int offset, int limit, string region = null)
        {
            string query = base.DefaultSelect
                + base.DefaultFrom
                + (region == null ? base.DefaultWhere : base.RegionWhere)
                + "and (question_entity.stata_export_caption like 'f3r2q5b%' or question_entity.stata_export_caption = 'f3r2nenaideno')\n"
                + base.DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new { questionnaireId, region, offset, limit },
                transaction: Transaction
            ) ?? Enumerable.Empty<RawInterviewData>();

            return CollectInterviews(rawData);
        }
    }
}
