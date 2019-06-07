using CapiControls.DAL.Common;
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

        public IEnumerable<RawInterviewData> GetF1R2Interviews(QueryParams parameters)
        {
            string query = base.DefaultSelect
                + base.DefaultFrom
                + (parameters.Region == null ? base.DefaultWhere : base.RegionWhere)
                + "and question_entity.stata_export_caption like 'f1r1q%'\n"
                + "order by summary.interviewid, question_entity.stata_export_caption, interview.rostervector\n"
                + base.DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new {
                    questionnaireId = parameters.QuestionnaireId,
                    region = parameters.Region,
                    offset = parameters.Offset,
                    limit = parameters.Limit
                },
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

        public async Task<bool> IsMemberDroppedOut(string interviewId, string section)
        {
            object parameters = new
            {
                interviewId,
                sectionSuffix = section.Split('_')[1]
            };

            string isMemberDroppedOutQuery = AnswerSelect
                + AnswerFrom
                + @"where summary.summaryid = @interviewId
    and question_entity.stata_export_caption = 'f1r1q4'
    and interview.rostervector = @sectionSuffix
limit 1";

            string isMemberDroppedOutAnswer = await Connection.QueryFirstOrDefaultAsync<string>(
                isMemberDroppedOutQuery,
                param: parameters);

            string isMemberDroppedOutAsHeadmanQuery = AnswerSelect
                + AnswerFrom
                + @"where summary.summaryid = @interviewId
    and question_entity.stata_export_caption = 'f1r1q3'
    and interview.rostervector = @sectionSuffix
limit 1";

            string isMemberDroppedOutAsHeadmanAnswer = await Connection.QueryFirstOrDefaultAsync<string>(
                isMemberDroppedOutAsHeadmanQuery,
                param: parameters);

            // 4 - ответ 'выбывший насовсем из домохозяйства'
            // 98 - ответ 'глава выбыл, умер'
            return (isMemberDroppedOutAnswer == "4" || isMemberDroppedOutAsHeadmanAnswer == "98"); 
        }

        public async Task<bool> IsHeadmanDroppedOut(string interviewId, string section)
        {
            string query = AnswerSelect
                + AnswerFrom
                + @"where summary.summaryid = @interviewId
    and question_entity.stata_export_caption = 'f1r1q3'
    and interview.rostervector = @sectionSuffix
limit 1";
            string answer = await Connection.QueryFirstAsync<string>(
                query,
                param: new { interviewId, sectionSuffix = section.Split('_')[1] });

            return answer == "98"; // 98 - ответ 'глава выбыл, умер'
        }

        public async Task<string> GetCurrentMemberName(string interviewId, string sectionSuffix)
        {
            string query = @"select value->>'Answer' from (
select interview.aslist as names_list
from readside.interviews as interview
    join readside.interviews_id as interview_id
        on interview.interviewid = interview_id.id
    join readside.interviewsummaries as summary
        on interview_id.interviewid = summary.interviewid
    join readside.questionnaire_entities as question_entity
        on interview.entityid = question_entity.id
where summary.summaryid = @interviewId
    and question_entity.stata_export_caption = 'name'
limit 1) member_names, jsonb_array_elements(member_names.names_list)
where value->>'Value' = @sectionSuffix";

            return await Connection.QueryFirstOrDefaultAsync<string>(
                query,
                param: new { interviewId, sectionSuffix = $"{sectionSuffix}.0" });
        }
    }
}
