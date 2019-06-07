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
        protected override string SectionNumber
        {
            get { return "2"; }
        }

        public F3R2unitsControl(
            IRemoteUnitOfWork uow,
            IInterviewService interviewService,
            IQuestionnaireService questionnaireService,
            IHostingEnvironment hostEnv) : base(uow, questionnaireService, interviewService, hostEnv)
        { }

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

            var rawInterviewsData = Uow.Form3Repository
                .GetF3R2UnitsInterviewsData(questionnaireId, offset, limit, region)
                .ToList();
            var interviews = InterviewService.CollectInterviews(rawInterviewsData);

            if (!(interviews.Count <= 0))
            {
                using (var file = DocX.Load(_reportFilePath))
                {
                    string productCode, unit, error;
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
                                error = $"{product.Name} (единицы измерения)";
                                base.WriteErrorToFile(file, interview.Id, error, SectionNumber);
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
