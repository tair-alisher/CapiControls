using CapiControls.Controls.Common;
using CapiControls.Controls.Interfaces;
using CapiControls.Data.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;

namespace CapiControls.Controls.Form3
{
    public class F3R2UnitsControl : BaseControl, IF3R2UnitsControl
    {
        private readonly IForm3Repository Repository;
        private readonly IInterviewRepository InterviewRepo;
        private string _outputCsvFilePath;

        public F3R2UnitsControl(IForm3Repository repository, IInterviewRepository interviewRepo, IHostingEnvironment hostEnv) : base(hostEnv)
        {
            Repository = repository;
            InterviewRepo = interviewRepo;
        }

        public string Execute(string questionnaireId)
        {
            _outputCsvFilePath = CreateReportCsvFile("F3R2Units");

            Execute(questionnaireId, 0, 1000);

            return _outputCsvFilePath;
        }

        private void Execute(string questionnaireId, int offset = 0, int limit = 1000)
        {
            ReadProductsFromFile(BuildFilePath(CatalogsDirectory, ProductInfoFileName));

            var interviews = Repository.GetF3R2UnitsInterviewsByQuestionnaire(questionnaireId, offset, limit);

            if (!(interviews.Count <= 0))
            {
                using (var file = File.AppendText(_outputCsvFilePath))
                {
                    string productCode;
                    string unit;
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
                                file.WriteLine("http://capi.stat.kg/Interview/Review/" + interview.Id + $"/Section/{questionData.QuestionSection.Replace("-", "")}"); // ; {product.Name};
                            }
                        }
                    }
                }

                Execute(questionnaireId, offset += 1000);
            }
        }
    }
}
