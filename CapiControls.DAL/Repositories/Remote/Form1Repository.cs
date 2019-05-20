using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Repositories;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CapiControls.DAL.Repositories.Remote
{
    internal class Form1Repository : InterviewRepository, IForm1Repository
    {
        public Form1Repository(IDbTransaction transaction) : base(transaction) { }

        public IEnumerable<RawInterviewData> GetF1R2Interviews(string questionnaireId, int offset, int limit, string region = null)
        {
            string query = base.DefaultSelect
                + base.DefaultFrom
                + (region == null ? base.DefaultWhere : base.RegionWhere)
                + "and (question_entity.stata_export_caption like 'f1r1q%' "
                + "or question_entity.stata_export_caption = 'dateF')\n"
                + "order by summary.interviewid, question_entity.stata_export_caption, interview.rostervector\n"
                + base.DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new { questionnaireId, region, offset, limit },
                transaction: Transaction
            ) ?? Enumerable.Empty<RawInterviewData>();

            return rawData;
        }

        public async Task<string> GetInterviewDate(string interviewId)
        {
            string query = AnswerSelect
                + AnswerFrom
                + @"where summary.summaryid = @interviewId
	and question_entity.stata_export_caption = 'dateF'
limit 1";

            return await Connection.QueryFirstOrDefaultAsync<string>(query, param: new { interviewId });
        }

        public async Task<string> GetMemberBirthDate(string interviewId, string section)
        {
            string query = AnswerSelect
                + AnswerFrom
                + @"where summary.summaryid = @interviewId
    and interview.rostervector = @sectionSuffix
	and question_entity.stata_export_caption = 'f1r1q6'
limit 1";

            return await Connection.QueryFirstOrDefaultAsync<string>(
                query,
                param: new { interviewId, sectionSuffix = section.Split('_')[1] });
        }

        public async Task<string> GetMemberMaritalStatus(string interviewId, string section)
        {
            string query = AnswerSelect
                + AnswerFrom
                + @"where summary.summaryid = @interviewId
    and interview.rostervector = @sectionSuffix
	and question_entity.stata_export_caption = 'f1r1q8'
limit 1";

            return await Connection.QueryFirstAsync<string>(
                query,
                param: new { interviewId, sectionSuffix = section.Split('_')[1] });
        }

        public async Task<bool> HasMemberSpouse(string interviewId)
        {
            string query = AnswerSelect
                + AnswerFrom
                + @"where summary.summaryid = @interviewId
    and question_entity.stata_export_caption = 'f1r1q3'
    and coalesce(
        interview.asstring
        , cast(interview.asint as varchar)
        , cast(interview.aslong as varchar)
        , cast(interview.asdouble as varchar)
        , cast(interview.asdatetime as varchar)
        , cast(interview.aslist as varchar)
        , cast(interview.asbool as varchar)
        , cast(interview.asintarray as varchar)
        , cast(interview.asintmatrix as varchar)
        , cast(interview.asgps as varchar)
        , cast(interview.asyesno as varchar)
        , cast(interview.asaudio as varchar)
        , cast(interview.asarea as varchar)
        , ''
    ) = '2'
    and interview.rostervector != '1'
limit 1";

            string spouse = await Connection.QueryFirstOrDefaultAsync<string>(
                query,
                param: new { interviewId });

            return !string.IsNullOrEmpty(spouse);
        }

        public async Task<bool> IsMemberAbsenceReasonAnswered(string interviewId, string section)
        {
            string query = AnswerSelect
                + AnswerFrom
                + @"where summary.summaryid = @interviewId
    and question_entity.stata_export_caption = 'f1r1q5'
    and interview.rostervector = @sectionSuffix
limit 1";

            string memberAbsenceReason = await Connection.QueryFirstOrDefaultAsync<string>(
                query,
                param: new { interviewId, sectionSuffix = section.Split('_')[1] });

            return !string.IsNullOrEmpty(memberAbsenceReason);
        }
    }
}
