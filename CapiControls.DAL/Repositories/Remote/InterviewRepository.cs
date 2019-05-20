using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Repositories;
using CapiControls.DAL.Repositories.Base;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CapiControls.DAL.Repositories.Remote
{
    internal class InterviewRepository : BaseRepository, IInterviewRepository
    {
        protected string DefaultSelect
        {
            get
            {
                return @"select
    summary.summaryid as InterviewId
    , summary.questionnaireidentity as QuestionnaireId
    , summary.teamleadname as Region
    , question_entity.stata_export_caption as QuestionCode
    , cast(question_entity.parentid as varchar) as QuestionSection
    , interview.rostervector as QuestionSectionSuffix
    , coalesce(
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
) as Answer
";
            }
        }

        protected string AnswerSelect
        {
            get
            {
                return @"select coalesce(
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
) as Answer
";
            }
        }

        protected string DefaultFrom
        {
            get
            {
                return @"from readside.interviews as interview
    join readside.interviews_id as interview_id
        on interview.interviewid = interview_id.id
    join readside.interviewsummaries as summary
        on interview_id.interviewid = summary.interviewid
    join readside.questionnaire_entities as question_entity
        on interview.entityid = question_entity.id
    join readside.interviewcommentaries as interview_info
        on summary.summaryid = interview_info.id
";
            }
        }

        protected string AnswerFrom
        {
            get
            {
                return @"from readside.interviews as interview
    join readside.interviews_id as interview_id
        on interview.interviewid = interview_id.id
    join readside.interviewsummaries as summary
        on interview_id.interviewid = summary.interviewid
    join readside.questionnaire_entities as question_entity
        on interview.entityid = question_entity.id
";
            }
        }

        protected string DefaultWhere
        {
            get
            {
                return @"where
    question_entity.stata_export_caption is not null
    and interview_info.isdeleted = false
    and interview_info.isapprovedbyhq = false
    and summary.wasrejectedbysupervisor = false
    and summary.questionnaireidentity = @questionnaireId
";
            }
        }

        protected string RegionWhere
        {
            get
            {
                return DefaultWhere + "and lower(summary.teamleadname) like concat(@region, '%')\n";
            }
        }

        protected string DefaultOrder
        {
            get
            {
                return "order by interview_id\n";
            }
        }

        protected string DefaultOffsetLimit
        {
            get
            {
                return "offset @offset limit @limit\n";
            }
        }

        public InterviewRepository(IDbTransaction transaction) : base(transaction) { }

        public IEnumerable<RawInterviewData> GetInterviewsDataByQuestionnaire(string questionnaireId, int offset, int limit)
        {
            string query = DefaultSelect + DefaultFrom + DefaultWhere + DefaultOrder + DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new { questionnaireId, offset, limit },
                transaction: Transaction
            ) ?? Enumerable.Empty<RawInterviewData>();

            return rawData;
        }

        public IEnumerable<RawInterviewData> GetInterviewsDataByQuestionnaireAndRegion(string questionnaireId, string region, int offset, int limit)
        {
            string query = DefaultSelect + DefaultFrom + RegionWhere + DefaultOrder + DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new { questionnaireId, region, offset, limit },
                transaction: Transaction
            ) ?? Enumerable.Empty<RawInterviewData>();

            return rawData;
        }
        
        public IEnumerable<RawInterviewData> GetInterviewsDataByQuestionnaireAndQuestionCode(string questionnaireId, string questionCode, int offset, int limit)
        {
            string query = DefaultSelect + DefaultFrom + DefaultWhere + "\nand question_entity.stata_export_caption = @questionCode\n" + DefaultOrder + DefaultOffsetLimit;

            var rawData = Connection.Query<RawInterviewData>(
                query,
                param: new { questionnaireId, questionCode, offset, limit },
                transaction: Transaction
            ) ?? Enumerable.Empty<RawInterviewData>();

            return rawData;
        }

        public string GetQuestionAnswerBySection(string interviewId, string questionCode, string section)
        {
            string sectionId = section.Split('_').Last();
            section = section.Split('_').First();
            string query = AnswerSelect
                + AnswerFrom
                + @"where
    question_entity.stata_export_caption = @questionCode
    and summary.summaryid = @interviewId
    and question_entity.parentid = @section
    and interview.rostervector = @sectionId
limit 1";

            return Connection.QueryFirst<string>(
                query,
                param: new { questionCode, interviewId, section = new Guid(section), sectionId },
                transaction: Transaction
            );
        }

        public string GetQuestionFirstAnswer(string interviewId, string questionCode)
        {
            string query = AnswerSelect
                + AnswerFrom
                + @"where
    question_entity.stata_export_caption = @questionCode
    and summary.summaryid = @interviewId
limit 1";

            return Connection.QueryFirst<string>(
                query,
                param: new { questionCode, interviewId },
                transaction: Transaction
            );
        }

        public string GetInterviewKey(string interviewId)
        {
            return Connection.QueryFirst<string>(
                @"select key from readside.interviewsummaries
where summaryid = @interviewId limit 1",
                param: new { interviewId },
                transaction: Transaction
            );
        }
    }
}
