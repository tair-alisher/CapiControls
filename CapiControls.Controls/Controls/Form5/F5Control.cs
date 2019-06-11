using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Interfaces.Form5;
using CapiControls.DAL.Common;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Controls.Controls.Form5
{
    public class F5Control : BaseControl, IF5Control
    {
        protected override string SectionNumber
        {
            get { return "Учет расходов на непрод. товары"; }
        }

        public F5Control(
            IRemoteUnitOfWork uow,
            IInterviewService interviewService,
            IQuestionnaireService questionnaireService,
            IHostingEnvironment hostEnv) : base(uow, questionnaireService, interviewService, hostEnv) { }

        public string Execute(string questionnaireId, string region = null)
        {
            base._reportFilePath = base.CreateReportFile("F5Units");
            base._questionnaireTitle = base.GetQuestionnaireTitle(questionnaireId);

            Execute(new QueryParams
            {
                QuestionnaireId = questionnaireId,
                Region = region
            });

            return _reportFilePath;
        }

        private void Execute(QueryParams parameters)
        {
            if (NonFoodItems == null || NonFoodItems.Count <= 0)
                ReadNonFoodItemsInfoFromFile(BuildFilePath(CatalogsDirectory, NeprodInfoFileName));

            var answers = Uow.Form5Repository.GetF5InterviewsData(parameters);

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

        private void CheckAnswers(List<F5AnswerData> answers, DocX file)
        {
            string error;
            foreach (var answer in answers)
            {
                var item = NonFoodItems.Where(i => i.Code == answer.ItemCode).FirstOrDefault();

                if (item != null)
                {
                    if (!item.Units.Contains(answer.ItemUnits))
                    {
                        error = $"{item.Name} (единицы измерения)";
                        base.WriteErrorToFile(file, answer.InterviewId, error, SectionNumber);
                    }

                    if (!item.Materials.Contains(answer.ItemMaterial))
                    {
                        error = $"{item.Name} (материал)";
                        base.WriteErrorToFile(file, answer.InterviewId, error, SectionNumber);
                    }

                    if (!item.Purposes.Contains(answer.ItemPurpose))
                    {
                        error = $"{item.Name} (для кого куплено)";
                        base.WriteErrorToFile(file, answer.InterviewId, error, SectionNumber);
                    }
                }
            }
        }
    }
}
