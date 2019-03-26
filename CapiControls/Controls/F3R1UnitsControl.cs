using CapiControls.Controls.Interfaces;
using CapiControls.Data.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CapiControls.Controls
{
    public class F3R1UnitsControl : BaseControl, IF3R1UnitsControl
    {
        private readonly IInterviewRepository InterviewRepository;
        private Dictionary<string, string[]> ProductCodesAndUnits;
        private string _outputCsvFilePath;

        public F3R1UnitsControl(IInterviewRepository interviewRepo, IHostingEnvironment hostEnv) : base(hostEnv)
        {
            InterviewRepository = interviewRepo;
        }

        public void Execute(string questionnaireId)
        {
            _outputCsvFilePath = BuildFilePath("F3R1Units-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".csv");
            File.Create(_outputCsvFilePath).Close();

            string questionCode = "f3r1q6";
            Execute(questionnaireId, questionCode);
        }

        private bool Execute(string questionnaireId, string questionCode, int offset = 0, int limit = 1000)
        {
            ReadProductCodesAndUnitsFromFile(BuildFilePath("Units.csv"));
            var interviews = InterviewRepository.GetF1R3InterviewsByQuestionnaire(questionnaireId, offset, limit);

            if (!(interviews.Count <= 0))
            {
                using (var file = File.AppendText(_outputCsvFilePath))
                {
                    foreach (var interview in interviews)
                    {
                        foreach (var questionData in interview.QuestionData)
                        {
                            if (questionData.QuestionCode == questionCode)
                            {
                                string productCode = InterviewRepository.GetQuestionAnswer(interview.Id, "tovKod");
                                string unit = questionData.Answer;

                                if (ProductCodesAndUnits.ContainsKey(productCode) && !ProductCodesAndUnits[productCode].Contains(unit))
                                {
                                    file.WriteLine("http://capi.stat.kg/Interview/Review/" + interview.Id);
                                }
                            }
                        }
                    }
                }

                Execute(questionnaireId, questionCode, offset += 1000);
            }

            return true;
        }

        private void ReadProductCodesAndUnitsFromFile(string fileName)
        {
            ProductCodesAndUnits = new Dictionary<string, string[]>();

            string filePath = BuildFilePath(fileName);
            var fileStream = new FileStream(filePath, FileMode.Open);
            using (var reader = new StreamReader(fileStream))
            {
                string[] lineParts = reader.ReadLine().Split(';');
                string productCode = lineParts[0];
                string[] units = lineParts[1].Split('/');

                ProductCodesAndUnits.Add(productCode, units);
            }
        }
    }
}
