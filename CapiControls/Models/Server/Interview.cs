using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Models.Server
{
    public class Interview
    {
        public string Id { get; set; }
        public string QuestionnaireId { get; set; }
        public List<QuestionData> QuestionData;

        public Interview() =>
            QuestionData = new List<QuestionData>();
    }

    public class QuestionData
    {
        public string QuestionSection { get; set; }
        public string QuestionCode { get; set; }
        public string Answer { get; set; }
    }

    public class RawInterviewData
    {
        public string InterviewId;
        public string QuestionnaireId;
        public string QuestionCode;
        public string QuestionSection;
        public string QuestionSectionSuffix;
        public string Answer;
    }
}
