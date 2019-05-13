using System.Collections.Generic;

namespace CapiControls.DAL.Entities
{
    public class Interview
    {
        public string Id { get; set; }
        public string QuestionnaireId { get; set; }
        public List<QuestionData> QuestionData { get; set; } = new List<QuestionData>();
    }

    public class QuestionData
    {
        public string QuestionSection { get; set; }
        public string QuestionCode { get; set; }
        public string Answer { get; set; }
    }

    public class RawInterviewData
    {
        public string InterviewId { get; set; }
        public string QuestionnaireId { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionSection { get; set; }
        public string QuestionSectionSuffix { get; set; }
        public string Answer { get; set; }
    }
}
