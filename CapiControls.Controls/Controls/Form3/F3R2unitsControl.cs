using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Common;
using CapiControls.Controls.Interfaces.Form3;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System.Linq;

namespace CapiControls.Controls.Controls.Form3
{
    public class F3R2unitsControl : BaseControl, IF3R2UnitsControl
    {
        private readonly IRemoteUnitOfWork _uow;
        private readonly IInterviewService _interviewService;

        public F3R2unitsControl(
            IRemoteUnitOfWork uow,
            IInterviewService interviewService,
            IQuestionnaireService questionnaireService,
            IHostingEnvironment hostEnv) : base(questionnaireService, hostEnv)
        {
            _uow = uow;
            _interviewService = interviewService;
        }

        public string Execute(string questionnaireId, string region = null)
        {
            _reportFilePath = CreateReportFile("F3R2Units");
            _questionnaireTitle = GetQuestionnaireTitle(questionnaireId);

            Execute(questionnaireId, region, 0, 1000);

            return _reportFilePath;
        }

        private void Execute(string questionnaireId, string region = null, int offset = 0, int limit = 1000)
        {
            if (Products == null || Products.Count <= 0)
                ReadProdInfoFromFile(BuildFilePath(CatalogsDirectory, ProdInfoFileName));

            var rawInterviewsData = _uow.Form3Repository
                .GetF3R2UnitsInterviewsData(questionnaireId, offset, limit, region)
                .ToList();
            var interviews = _interviewService.CollectInterviews(rawInterviewsData);

            if (!(interviews.Count <= 0))
            {
                using (var file = DocX.Load(_reportFilePath))
                {
                    string productCode, unit, hhCode, key;
                    Product product;

                    foreach (var interview in interviews)
                    {
                        foreach (var questionData in interview.QuestionData)
                        {
                            productCode = questionData.QuestionSection.Split('_')[1];
                            unit = questionData.Answer;
                            product = Products.Where(p => p.Code == productCode).FirstOrDefault();

                            if (product != null && !product.Units.Contains(unit))
                            {
                                hhCode = _interviewService.GetQuestionFirstAnswer(interview.Id, "hhCode");
                                key = _interviewService.GetInterviewKey(interview.Id);

                                file.InsertParagraph($"{FormString}: {_questionnaireTitle}.");
                                file.InsertParagraph($"{IdentifierString}: {key}.");
                                file.InsertParagraph($"{SectionString}: 2.");
                                file.InsertParagraph($"{HouseholdCodeString}: {hhCode}.");
                                file.InsertParagraph($"{ErrorString}: {product.Name} (единицы измерения).");
                                file.InsertParagraph();
                            }
                        }
                    }

                    file.Save();
                }

                Execute(questionnaireId, region, offset += 1000);
            }
        }
    }
}
