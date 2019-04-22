using CapiControls.Controls.Common;
using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CapiControls.Controls
{
    public abstract class BaseControl
    {
        protected const string ReportsDirectory = "Reports";
        protected const string CatalogsDirectory = "Catalogs";

        protected const string FormString = "Форма";
        protected const string IdentifierString = "Идентификатор";
        protected const string SectionString = "Раздел";
        protected const string HouseholdCodeString = "Код домохозяйства";
        protected const string ErrorString = "Ошибка";

        protected const string ProdInfoFileName = "ProdUnits.txt";

        protected readonly IPaginatedRepository<Questionnaire> QuestionnaireRepo;
        private readonly IHostingEnvironment HostingEnvironment;

        protected List<Product> Products;

        public BaseControl(IPaginatedRepository<Questionnaire> questionnaireRepo, IHostingEnvironment hostEnv)
        {
            QuestionnaireRepo = questionnaireRepo;
            HostingEnvironment = hostEnv;
        }

        protected string BuildFilePath(string directory, string fileName)
        {
            string rootDir = HostingEnvironment.ContentRootPath;
            return Path.Combine(rootDir, "Files", directory, fileName);
        }

        protected virtual void ReadProdInfoFromFile(string filePath)
        {
            Products = new List<Product>();

            using (var fileStream = File.OpenRead(filePath))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string line;
                    string code;
                    string name;
                    string[] units;
                    Product product;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] lineParts = line.Split(';');

                        code = lineParts[0];
                        name = lineParts[1];
                        units = lineParts[3].Split('/');
                        product = new Product(code, name, units);

                        Products.Add(product);
                    }
                    
                }
            }
        }

        protected string GetQuestionnaireTitle(string identifier)
        {
            return QuestionnaireRepo.GetAll().Where(q => q.Identifier == identifier).First().Title;
        }

        protected string CreateReportFile(string fileName)
        {
            string filePath = BuildFilePath(ReportsDirectory, $"{fileName}-" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm") + ".docx");
            DocX.Create(filePath).Save();

            return filePath;
        }
    }
}
