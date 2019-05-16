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

        public IEnumerable<RawInterviewData> GetF3R1UnitsInterviewsDataByQuestionnaire(string questionnaireId, int offset, int limit, string region = null)
        {
            string query = base.DefaultSelect
                + base.DefaultFrom
                + (region == null ? base.DefaultWhere : base.RegionWhere)
                + "and question_entity.stata_export_caption = 'f3r1q6'\n"
                + base.DefaultOrder
                + base.DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new { questionnaireId, region, offset, limit },
                transaction: Transaction
            ) ?? Enumerable.Empty<RawInterviewData>();

            return rawData;
        }

        public IEnumerable<RawInterviewData> GetF3R2UnitsInterviewsDataByQuestionnaire(string questionnaireId, int offset, int limit, string region = null)
        {
            string query = base.DefaultSelect
                + base.DefaultFrom
                + (region == null ? base.DefaultWhere : base.RegionWhere)
                + "and (question_entity.stata_export_caption like 'f3r2q5b%' or question_entity.stata_export_caption = 'f3r2nenaideno')\n"
                + base.DefaultOrder
                + base.DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new { questionnaireId, region, offset, limit },
                transaction: Transaction
            ) ?? Enumerable.Empty<RawInterviewData>();

            return rawData;
        }
    }
}
