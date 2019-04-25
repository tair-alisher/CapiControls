using CapiControls.BLL.DTO;
using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Common;
using CapiControls.Controls.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace CapiControls.Controls.Controls
{
    public class BaseControl
    {
        protected const string ReportsDirectory = "Reports";
        protected const string CatalogsDirectory = "Catalogs";

        protected const string FormString = "Форма";
        protected const string IdentifierSring = "Идентификатор";
        protected const string SectionString = "Раздел";
        protected const string HouseholdCodeString = "Код домохозяйства";
        protected const string ErrorString = "Ошибка";

        protected const string ProdInfoFileName = "ProdUnits.txt";

        protected readonly IQuestionnaireService QuestionnaireService;
        private readonly IHostingEnvironment _hostEnv;

        protected List<Product> Products;

        public BaseControl(IQuestionnaireService questionnaireService, IHostingEnvironment hostEnv)
        {
            QuestionnaireService = questionnaireService;
            _hostEnv = hostEnv;
        }

        protected string BuildFilePath(string directory, string fileName)
        {
            string rootDir = _hostEnv.ContentRootPath;
            return Path.Combine(rootDir, "Files", directory, fileName);
        }

        protected virtual void ReadProdInfoFromFile(string filePath)
        {
            Products = new List<Product>();

            using (var fileStream = File.OpenRead(filePath))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string line, code, name;
                    string[] lineParts, units;
                    Product product;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineParts = line.Split(';');

                        code = lineParts[0];
                        name = lineParts[1];
                        units = lineParts[3].Split('/');
                        product = new Product(code, name, units);

                        Products.Add(product);
                    }
                }
            }
        }
    }
}
