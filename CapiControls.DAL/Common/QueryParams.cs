namespace CapiControls.DAL.Common
{
    public class QueryParams
    {
        public string QuestionnaireId { get; set; }
        public string Region { get; set; }
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 1000;
    }
}
