using CapiControls.Data.Interfaces;
using CapiControls.Models.Server;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CapiControls.Data.Repositories.Server
{
    public class InterviewRepository : BaseRepository, IInterviewRepository
    {

        public InterviewRepository(IConfiguration configuration) : base(configuration, "server.readside") { }

        protected string BaseQuery
        {
            get
            {
                return @"
                    select
                        summary.summaryid as InterviewId
                        , summary.questionnaireidentity as QuestionnaireId
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
                            , @no_answer
                        ) as Answer
                    from
                        readside.interviews as interview
                        join
                            readside.interviews_id as interview_id
                        on
                            interview.interviewid = interview_id.id
                        join
                            readside.interviewsummaries as summary
                        on
                            interview_id.interviewid = summary.interviewid
                        join
                            readside.questionnaire_entities as question_entity
                        on
                            interview.entityid = question_entity.id
                        join
                            readside.interviewcommentaries as interview_info
                        on
                            summary.summaryid = interview_info.id
                    where
                        question_entity.stata_export_caption is not null
                        and interview_info.isdeleted = false
                        and interview_info.isapprovedbyhq = false
                        and summary.wasrejectedbysupervisor = false
                        and summary.questionnaireidentity = @questionnaire_id
                ";
            }
        }

        public List<Interview> GetInterviewsByQuestionnaire(string questionnaireId, int offset, int limit)
        {
            string query = BaseQuery + @"
                order by interview_id
                offset @_offset
                limit @_limit
                ";

            IEnumerable<RawInterviewData> rawData = Enumerable.Empty<RawInterviewData>();
            using (IDbConnection connection = Connection)
            {
                rawData = connection.Query<RawInterviewData>(query, new
                {
                    no_answer = "",
                    questionnaire_id = questionnaireId,
                    _offset = offset,
                    _limit = limit
                });
            }

            return CollectInterviews(rawData);
        }

        public List<Interview> CollectInterviews(IEnumerable<RawInterviewData> rawData)
        {
            List<Interview> interviews = new List<Interview>();
            Interview interview = null;
            QuestionData questionData = null;
            foreach (var row in rawData)
            {
                bool interviewAlreadyAdded = interviews.Where(i => i.Id == row.InterviewId).Count() > 0;

                if (interviewAlreadyAdded)
                {
                    interview = interviews.Where(i => i.Id == row.InterviewId).First();
                    questionData = new QuestionData
                    {
                        QuestionSection = row.QuestionSectionSuffix.Length > 0 ?
                            $"{row.QuestionSection}_{row.QuestionSectionSuffix}" :
                            row.QuestionSection,
                        QuestionCode = row.QuestionCode,
                        Answer = row.Answer
                    };
                    interview.QuestionData.Add(questionData);
                }
                else
                {
                    interview = new Interview
                    {
                        Id = row.InterviewId,
                        QuestionnaireId = row.QuestionnaireId
                    };
                    questionData = new QuestionData
                    {
                        QuestionSection = row.QuestionSectionSuffix.Length > 0 ?
                            $"{row.QuestionSection}_{row.QuestionSectionSuffix}" :
                            row.QuestionSection,
                        QuestionCode = row.QuestionCode,
                        Answer = row.Answer
                    };
                    interview.QuestionData.Add(questionData);

                    interviews.Add(interview);
                }
            }

            return interviews;
        }

        public List<Interview> GetInterviewsByQuestionnaireAndQuestionCode(string questionnaireId, string questionCode, int offset, int limit)
        {
            string query = BaseQuery + @"
                        and question_entity.stata_export_caption = @question_code
                    order by interview_id
                    offset @_offset
                    limit @limit
                ";

            IEnumerable<RawInterviewData> rawData = Enumerable.Empty<RawInterviewData>();
            using (var connection = Connection)
            {
                rawData = connection.Query<RawInterviewData>(query, new
                {
                    no_answer = "",
                    questionnaire_id = questionnaireId,
                    question_code = questionCode,
                    _offset = offset,
                    _limit = limit
                });
            }

            return CollectInterviews(rawData);
        }

        public string GetQuestionAnswerBySection(string interviewId, string questionCode, string section)
        {
            string sectionId = section.Split('_').Last();
            section = section.Split('_').First();
            string query = @"
                    select
                        coalesce(
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
                            , @noAnswer
                        ) as Answer
                    from
                        readside.interviews as interview
                        join
                            readside.interviews_id as interview_id
                        on
                            interview.interviewid = interview_id.id
                        join
                            readside.interviewsummaries as summary
                        on
                            interview_id.interviewid = summary.interviewid
                        join
                            readside.questionnaire_entities as question_entity
                        on
                            interview.entityid = question_entity.id
                    where
                        question_entity.stata_export_caption = @questionCode
                        and summary.summaryid = @interviewId
                        and question_entity.parentid = @section
                        and interview.rostervector = @sectionId
                    limit 1
                ";

            string answer = "";
            using (var connection = Connection)
            {
                answer = connection.QueryFirst<string>(query, new
                {
                    noAnswer = "",
                    questionCode,
                    interviewId,
                    section = new Guid(section),
                    sectionId
                });
            }

            return answer;
        }

        public string GetQuestionFirstAnswer(string interviewId, string questionCode)
        {
            string query = @"
                    select
                        coalesce(
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
                            , @noAnswer
                        ) as Answer
                    from
                        readside.interviews as interview
                        join
                            readside.interviews_id as interview_id
                        on
                            interview.interviewid = interview_id.id
                        join
                            readside.interviewsummaries as summary
                        on
                            interview_id.interviewid = summary.interviewid
                        join
                            readside.questionnaire_entities as question_entity
                        on
                            interview.entityid = question_entity.id
                    where
                        question_entity.stata_export_caption = @questionCode
                        and summary.summaryid = @interviewId
                    limit 1
                ";

            string answer = "";
            using (var connection = Connection)
            {
                answer = connection.QueryFirst<string>(query, new
                {
                    noAnswer = "",
                    questionCode,
                    interviewId
                });
            }

            return answer;
        }
    }
}
