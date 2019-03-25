using CapiControls.Models.Local;

namespace CapiControls.Data.Interfaces
{
    public interface IQuestionnaireRepository : IRepository<Questionnaire>, IPaginated<Questionnaire>
    {
    }
}
