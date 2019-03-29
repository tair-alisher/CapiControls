using CapiControls.Controls.Common;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace CapiControls.Controls
{
    public abstract class BaseControl
    {
        protected const string ReportsDirectory = "Reports";
        protected const string CatalogsDirectory = "Catalogs";

        protected const string ProductInfoFileName = "Units.txt";

        private readonly IHostingEnvironment HostingEnvironment;

        protected List<Product> Products;

        public BaseControl(IHostingEnvironment hostEnv)
        {
            HostingEnvironment = hostEnv;
        }

        protected string BuildFilePath(string directory, string fileName)
        {
            string rootDir = HostingEnvironment.ContentRootPath;
            return Path.Combine(rootDir, "Files", directory, fileName);
        }

        protected virtual void ReadProductsFromFile(string filePath)
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
                        units = lineParts[2].Split('/');
                        product = new Product(code, name, units);

                        Products.Add(product);
                    }
                    
                }
            }
        }

        protected string CreateReportCsvFile(string fileName)
        {
            string filePath = BuildFilePath(ReportsDirectory, $"{fileName}-" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm") + ".csv");
            File.Create(filePath).Close();

            return filePath;
        }
    }
}
