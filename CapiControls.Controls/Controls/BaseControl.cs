using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Common;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CapiControls.Controls.Controls
{
    public abstract class BaseControl
    {
        protected abstract string SectionNumber { get; }
        internal List<Product> Products = null;
        internal List<NonFoodItem> NonFoodItems = null;

        protected string _reportFilePath;
        protected string _questionnaireTitle;

        protected const string ReportsDirectory = "Reports";
        protected const string CatalogsDirectory = "Catalogs";

        protected const string ProdInfoFileName = "ProdUnits.txt";
        protected const string NeprodInfoFileName = "Neprod.txt";

        protected readonly IRemoteUnitOfWork Uow;
        protected readonly IQuestionnaireService QuestionnaireService;
        protected readonly IInterviewService InterviewService;
        private readonly IHostingEnvironment _hostEnv;

        public BaseControl(
            IRemoteUnitOfWork uow,
            IQuestionnaireService questionnaireService,
            IInterviewService interviewService,
            IHostingEnvironment hostEnv)
        {
            Uow = uow;
            QuestionnaireService = questionnaireService;
            InterviewService = interviewService;
            _hostEnv = hostEnv;
        }

        protected string BuildFilePath(string directory, string fileName)
        {
            string rootDir = _hostEnv.ContentRootPath;
            return Path.Combine(rootDir, "Files", directory, fileName);
        }

        protected void ReadProdInfoFromFile(string filePath)
        {
            Products = new List<Product>();

            using (var fileStream = File.OpenRead(filePath))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string line;
                    string[] lineParts;
                    Product product;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineParts = line.Split(';');
                        product = new Product
                        {
                            Code = lineParts[0],
                            Name = lineParts[1],
                            GskpCode = lineParts[2],
                            Units = lineParts[3].Split('/')
                        };

                        Products.Add(product);
                    }
                }
            }
        }

        protected void ReadNonFoodItemsInfoFromFile(string filePath)
        {
            NonFoodItems = new List<NonFoodItem>();

            using (var fileStream = File.OpenRead(filePath))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string line;
                    string[] lineParts;
                    NonFoodItem item;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineParts = line.Split(';');
                        item = new NonFoodItem
                        {
                            Code = lineParts[0],
                            Name = lineParts[1],
                            GskpCode = lineParts[2],
                            Units = lineParts[3].Split(null),
                            Materials = lineParts[5].Split(null),
                            Purposes = lineParts[7].Split(null)
                        };

                        NonFoodItems.Add(item);

                    }
                }
            }
        }

        protected string GetQuestionnaireTitle(string identifier)
        {
            return QuestionnaireService.GetQuestionnaires()
                .Where(q => q.Identifier == identifier)
                .First()
                .Title;
        }

        protected string CreateReportFile(string fileName)
        {
            string filePath = BuildFilePath(ReportsDirectory, $"{fileName}-" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm") + ".docx");
            DocX.Create(filePath).Save();

            return filePath;
        }

        protected void WriteErrorToFile(DocX file, string interviewId, string error, string sectionNumber)
        {
            string hhCode = InterviewService.GetQuestionFirstAnswer(interviewId, "hhCode");
            string key = InterviewService.GetInterviewKey(interviewId);

            file.InsertParagraph($"Форма: {_questionnaireTitle}.");
            file.InsertParagraph($"Идентификатор: {key}.");
            file.InsertParagraph($"Раздел: {sectionNumber}.");
            file.InsertParagraph($"Код домохозяйства: {hhCode}.");
            file.InsertParagraph($"Ошибка: {error}.");
            file.InsertParagraph();
        }
    }
}
