using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Interfaces.Form3;
using CapiControls.DAL.Common;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System.Collections.Generic;
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
            
            var interviews = InterviewService.CollectInterviews(
                Uow.Form3Repository.GetF3R2UnitsInterviewsData(parameters)
            );

            if (interviews.Count <= 0)
                return;

            using (var file = DocX.Load(_reportFilePath))
            {
                CheckInterviews(interviews, file);
                file.Save();
            }

            parameters.Offset += 1000;
            Execute(parameters);
        }

        private void CheckInterviews(List<InterviewDTO> interviews, DocX file)
        {
            foreach (var interview in interviews)
                CheckInterview(interview, file);
        }

        private void CheckInterview(InterviewDTO interview, DocX file)
        {
            foreach (var questionData in interview.QuestionData)
                CheckAnswer(interview.Id, questionData, file);
        }

        private void CheckAnswer(string interviewId, QuestionDataDTO questionData, DocX file)
        {
            string productCode = questionData.QuestionSection.Split('_')[1];
            string unit = questionData.Answer;
            var product = Products.Where(p => p.Code == productCode).FirstOrDefault();

            if (product != null && !product.Units.Contains(unit))
            {
                string error = $"{product.Name} (единицы измерения)";
                base.WriteErrorToFile(file, interviewId, error, SectionNumber);
            }
        }
    }
}
