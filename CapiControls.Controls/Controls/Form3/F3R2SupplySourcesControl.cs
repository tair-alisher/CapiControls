using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Common;
using CapiControls.Controls.Interfaces.Form3;
using CapiControls.DAL.Common;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CapiControls.Controls.Controls.Form3
{
    public class F3R2SupplySourcesControl : BaseControl, IF3R2SupplySourcesControl
    {
        protected override string SectionNumber
        {
            get { return "2"; }
        }

        private string _supplySourcesCodesFileName = "SupplySources_CommonProductCodes.txt";
        private Dictionary<string[], string[]> _productsSupplySources;

        public F3R2SupplySourcesControl(
            IRemoteUnitOfWork uow,
            IInterviewService interviewService,
            IQuestionnaireService questionnaireSerivce,
            IHostingEnvironment hostEnv) : base(uow, questionnaireSerivce, interviewService, hostEnv)
        { }

        public string Execute(string questionnaireId, string region = null)
        {
            _reportFilePath = CreateReportFile("F3R2SupplySources");
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

            if (_productsSupplySources == null || _productsSupplySources.Count <= 0)
                ReadProductsSupplySourcesFromFile(BuildFilePath(CatalogsDirectory, _supplySourcesCodesFileName));

            var interviews = InterviewService.CollectInterviews(
                Uow.Form3Repository.GetF3R2SupplySourcesInterviewsData(parameters)
            );

            if (interviews.Count > 0)
            {
                using (var file = DocX.Load(_reportFilePath))
                {
                    string productCode, supplySource, error;
                    string[] productSupplyCodes;
                    Product product;

                    foreach (var interview in interviews)
                    {
                        foreach (var questionData in interview.QuestionData)
                        {
                            productCode = questionData.QuestionSection.Split('_')[1];
                            supplySource = questionData.Answer;
                            product = Products.Where(p => p.Code == productCode).FirstOrDefault();

                            if (product != null)
                            {
                                productSupplyCodes = _productsSupplySources.Where(sp => sp.Value.Contains(product.GskpCode)).FirstOrDefault().Key;

                                if (productSupplyCodes.Count() > 0 && !productSupplyCodes.Contains(supplySource))
                                {
                                    error = $"{product.Name} (источник поступления)";
                                    base.WriteErrorToFile(file, interview.Id, error, SectionNumber);
                                }
                            }
                        }
                    }

                    file.Save();
                }

                parameters.Offset += 1000;
                Execute(parameters);
            }
        }

        private void ReadProductsSupplySourcesFromFile(string filePath)
        {
            _productsSupplySources = new Dictionary<string[], string[]>();

            using (var fileStream = File.OpenRead(filePath))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string line;
                    string[] supplyAndProductCodes;

                    while ((line = reader.ReadLine()) != null)
                    {
                        supplyAndProductCodes = line.Split(';');

                        _productsSupplySources.Add(
                            supplyAndProductCodes[0].Split('/'), // supply codes
                            supplyAndProductCodes[1].Split('/') // product codes
                        );
                    }
                }
            }
        }
    }
}
