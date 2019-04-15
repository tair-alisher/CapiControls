using CapiControls.Controls.Common;
using CapiControls.Controls.Interfaces;
using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System;
using System.Linq;

namespace CapiControls.Controls.Form3
{
    public class F3R2UnitsControl : BaseControl, IF3R2UnitsControl
    {
        private readonly IForm3Repository Repository;
        private readonly IInterviewRepository InterviewRepo;
        private string _outputCsvFilePath;
        private string _questionnaireTitle;

        public F3R2UnitsControl(IForm3Repository repository, IInterviewRepository interviewRepo, IPaginatedRepository<Questionnaire> questionnaireRepo, IHostingEnvironment hostEnv) : base(questionnaireRepo, hostEnv)
        {
            Repository = repository;
            InterviewRepo = interviewRepo;
        }

        public string Execute(string questionnaireId, string region)
        {
            _outputCsvFilePath = CreateReportFile("F3R2Units");
            _questionnaireTitle = GetQuestionnaireTitle(questionnaireId);

            Execute(questionnaireId, region, 0, 1000);

            return _outputCsvFilePath;
        }

        private void Execute(string questionnaireId, string region = null, int offset = 0, int limit = 1000)
        {
            ReadProductsFromFile(BuildFilePath(CatalogsDirectory, ProductInfoFileName));

            var interviews = Repository.GetF3R2UnitsInterviewsByQuestionnaire(questionnaireId, offset, limit, region);

            if (!(interviews.Count <= 0))
            {
                using (var file = DocX.Load(_outputCsvFilePath))
                {
                    string productCode;
                    string unit;
                    Product product;
                    string hhCode;
                    string key;
                    foreach (var interview in interviews)
                    {
                        foreach (var questionData in interview.QuestionData)
                        {
                            productCode = questionData.QuestionSection.Split('_')[1];
                            unit = questionData.Answer;

                            product = Products.Where(p => p.Code == productCode).FirstOrDefault();
                            if (product != null && !product.Units.Contains(unit))
                            {
                                hhCode = InterviewRepo.GetQuestionFirstAnswer(interview.Id, "hhCode");
                                key = InterviewRepo.GetInterviewKey(interview.Id);

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
