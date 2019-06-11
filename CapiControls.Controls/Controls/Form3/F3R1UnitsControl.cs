using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Common;
using CapiControls.Controls.Interfaces.Form3;
using CapiControls.DAL.Common;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Controls.Controls.Form3
{
    public class F3R1UnitsControl : BaseControl, IF3R1UnitsControl
    {
        protected override string SectionNumber
        {
            get { return "1"; }
        }

        public F3R1UnitsControl(
            IRemoteUnitOfWork uow,
            IInterviewService interviewService,
            IQuestionnaireService questionnaireService,
            IHostingEnvironment hostEnv) : base(uow, questionnaireService, interviewService, hostEnv)
        { }

        public string Execute(string questionnaireId, string region = null)
        {
            _reportFilePath = CreateReportFile("F3R1Units");
            _questionnaireTitle = GetQuestionnaireTitle(questionnaireId);

            Execute(new QueryParams
            {
                QuestionnaireId = questionnaireId,
                Region = region
            });

            return _reportFilePath;
        }

        private void Execute(QueryParams parameters)
        {
            if (Products == null || Products.Count <= 0)
                ReadProdInfoFromFile(BuildFilePath(CatalogsDirectory, ProdInfoFileName));

            var answers = Uow.Form3Repository.GetF3R1InterviewsData(parameters);

            if (answers.Count <= 0)
                return;

            using (var file = DocX.Load(_reportFilePath))
            {
                CheckAnswers(answers, file);
                file.Save();
            }

            parameters.Offset += 1000;
            Execute(parameters);
        }

        private void CheckAnswers(List<F3AnswerData> answers, DocX file)
        {
            string error;
            Product product;
            foreach (var answer in answers)
            {
                product = Products.Where(p => p.Code == answer.ProductCode).FirstOrDefault();
                if (product != null && !product.Units.Contains(answer.ProductUnits))
                {
                    error = $"{product.Name} (единицы измерения)";
                    base.WriteErrorToFile(file, answer.InterviewId, error, SectionNumber);
                }
            }
        }
    }
}
