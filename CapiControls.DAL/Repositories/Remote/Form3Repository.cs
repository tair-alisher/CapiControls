﻿using CapiControls.DAL.Common;
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

        public List<F3AnswerData> GetF3R1InterviewsData(QueryParams parameters)
        {
            string query = $@"select
    summary.summaryid as InterviewId
    , summary.teamleadname as Region
    , coalesce(
        interview.asstring
        , cast(interview.asint as varchar)
        , ''
    ) as ProductCode,
	(
    select coalesce(
        sub_interview.asstring
        , cast(sub_interview.asint as varchar)
        , ''
    )
    from
    readside.interviews as sub_interview
    join readside.interviews_id as sub_interview_id
    on sub_interview.interviewid = sub_interview_id.id
    join readside.interviewsummaries as sub_summary
    on sub_interview_id.interviewid = sub_summary.interviewid
    join readside.questionnaire_entities as sub_question_entity
    on sub_interview.entityid = sub_question_entity.id
    join readside.interviewcommentaries as sub_interview_info
    on sub_summary.summaryid = sub_interview_info.id
    where sub_summary.interviewid = summary.interviewid
	    and sub_question_entity.stata_export_caption = 'f3r1q6'
	    and sub_question_entity.parentid = question_entity.parentid
	    and sub_interview.rostervector = interview.rostervector
    limit 1
    ) as ProductUnits
    from
    readside.interviews as interview
    join readside.interviews_id as interview_id
    on interview.interviewid = interview_id.id
    join readside.interviewsummaries as summary
    on interview_id.interviewid = summary.interviewid
    join readside.questionnaire_entities as question_entity
    on interview.entityid = question_entity.id
    join readside.interviewcommentaries as interview_info
    on summary.summaryid = interview_info.id
    where
    question_entity.stata_export_caption is not null
    and interview_info.isdeleted = false
    and interview_info.isapprovedbyhq = false
    and summary.wasrejectedbysupervisor = false
    and summary.questionnaireidentity = @questionnaireId
    and (question_entity.stata_export_caption like 'tovKod')
    {(parameters.Region != null ? "and lower(summary.teamleadname) like concat(@region, '%')" : "")}
    order by summary.interviewid
    offset @offset
    limit @limit
";

            var data = Connection.Query<F3AnswerData>(
                query,
                param: new {
                    questionnaireId = parameters.QuestionnaireId,
                    region = parameters.Region,
                    offset = parameters.Offset,
                    limit = parameters.Limit
                },
                transaction: Transaction
            ) ?? Enumerable.Empty<F3AnswerData>();

            return data.ToList();
        }

        public List<F3AnswerData> GetF3R2InterviewsData(QueryParams parameters)
        {
            string query = $@"
select
    summary.summaryid as InterviewId
    , summary.teamleadname as Region
	,(
    select sub_interview.rostervector
    from
    readside.interviews as sub_interview
    join readside.interviews_id as sub_interview_id
    on sub_interview.interviewid = sub_interview_id.id
    join readside.interviewsummaries as sub_summary
    on sub_interview_id.interviewid = sub_summary.interviewid
    join readside.questionnaire_entities as sub_question_entity
    on sub_interview.entityid = sub_question_entity.id
    join readside.interviewcommentaries as sub_interview_info
    on sub_summary.summaryid = sub_interview_info.id
    where sub_summary.interviewid = summary.interviewid
	    and sub_question_entity.parentid = question_entity.parentid
	    and sub_interview.rostervector = interview.rostervector
    limit 1
    ) as ProductCode
    , coalesce(
        interview.asstring
        , cast(interview.asint as varchar)
        , ''
    ) as ProductUnits,
	(
    select coalesce(
        sub_interview.asstring
        , cast(sub_interview.asint as varchar)
        , ''
    )
    from
    readside.interviews as sub_interview
    join readside.interviews_id as sub_interview_id
    on sub_interview.interviewid = sub_interview_id.id
    join readside.interviewsummaries as sub_summary
    on sub_interview_id.interviewid = sub_summary.interviewid
    join readside.questionnaire_entities as sub_question_entity
    on sub_interview.entityid = sub_question_entity.id
    join readside.interviewcommentaries as sub_interview_info
    on sub_summary.summaryid = sub_interview_info.id
    where sub_summary.interviewid = summary.interviewid
	    and sub_question_entity.stata_export_caption like 'f3r2q6b%'
	    and sub_question_entity.parentid = question_entity.parentid
	    and sub_interview.rostervector = interview.rostervector
    limit 1
    ) as ProductSupplySource
    from
    readside.interviews as interview
    join readside.interviews_id as interview_id
    on interview.interviewid = interview_id.id
    join readside.interviewsummaries as summary
    on interview_id.interviewid = summary.interviewid
    join readside.questionnaire_entities as question_entity
    on interview.entityid = question_entity.id
    join readside.interviewcommentaries as interview_info
    on summary.summaryid = interview_info.id
    where
    question_entity.stata_export_caption is not null
    and interview_info.isdeleted = false
    and interview_info.isapprovedbyhq = false
    and summary.wasrejectedbysupervisor = false
    and summary.questionnaireidentity = @questionnaireId
    and (question_entity.stata_export_caption like 'f3r2q5b%')
    {(parameters.Region != null ? "and lower(summary.teamleadname) like concat(@region, '%')" : "")}
    order by summary.interviewid
    offset @offset
    limit @limit";

            var data = Connection.Query<F3AnswerData>(
                query,
                param: new
                {
                    questionnaireId = parameters.QuestionnaireId,
                    region = parameters.Region,
                    offset = parameters.Offset,
                    limit = parameters.Limit
                },
                transaction: Transaction
            ) ?? Enumerable.Empty<F3AnswerData>();

            return data.ToList();
        }
    }
}
