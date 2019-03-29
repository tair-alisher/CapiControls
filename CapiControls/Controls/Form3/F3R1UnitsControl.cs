using CapiControls.Controls.Common;
using CapiControls.Controls.Interfaces;
using CapiControls.Data.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;

namespace CapiControls.Controls.Form3
{
    public class F3R1UnitsControl : BaseControl, IF3R1UnitsControl
    {
        private readonly IForm3Repository Repository;
        private readonly IInterviewRepository InterviewRepo;
        private string _outputCsvFilePath;

        public F3R1UnitsControl(IForm3Repository repository, IInterviewRepository interviewRepo, IHostingEnvironment hostEnv) : base(hostEnv)
        {
            Repository = repository;
            InterviewRepo = interviewRepo;
        }

        public string Execute(string questionnaireId)
        {
            // Создается файл отчета, в который будут записаны результаты проверки
            _outputCsvFilePath = CreateReportCsvFile("F3R1Units");

            Execute(questionnaireId, 0, 1000);

            return _outputCsvFilePath;
        }

        private bool Execute(string questionnaireId, int offset = 0, int limit = 1000)
        {
            // Считываются данные о продуктах из файла-справочника формата .txt
            // Код, имя и единицы измерения продукта в переменную Products
            ReadProductsFromFile(BuildFilePath(CatalogsDirectory, ProductInfoFileName));

            // Выбирется 1000 записей, относящихся к первому разделу формы 3 и собираются/группируются в интервью
            var interviews = Repository.GetF3R1UnitsInterviewsByQuestionnaire(questionnaireId, offset, limit);

            // Если есть интервью, выполняется проверка данных
            if (!(interviews.Count <= 0))
            {
                using (var file = File.AppendText(_outputCsvFilePath))
                {
                    string productCode;
                    string unit;
                    Product product;
                    // Проход по каждому инетрвью
                    foreach (var interview in interviews)
                    {
                        // Проход по каждому коду вопроса с его ответом
                        foreach (var questionData in interview.QuestionData)
                        {
                            // Выбирается ответ на вопрос о коде товара для конкретного ответа на вопрос о единицах измерения
                            // Они имеют одинаковые секции
                            productCode = InterviewRepo.GetQuestionAnswerBySection(interview.Id, "tovKod", questionData.QuestionSection);
                            unit = questionData.Answer;

                            // Выбирается информация о продукте по его коду
                            product = Products.Where(p => p.Code == productCode).FirstOrDefault();
                            // Если информация о продукте имеется,
                            // и ответ на вопрос о единицах измерения не является допустимым для этого продукта
                            // информация об имени товара и номере интервью записывается в отчетный файл.
                            if (product != null && !product.Units.Contains(unit))
                            {
                                file.WriteLine("http://capi.stat.kg/Interview/Review/" + interview.Id + $"/Section/{questionData.QuestionSection.Replace("-", "")}"); //; {product.Name};
                            }
                        }
                    }
                }

                Execute(questionnaireId, offset += 1000);
            }

            return true;
        }
    }
}
