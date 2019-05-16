using CapiControls.BLL.Exceptions;
using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Interfaces.Form1;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CapiControls.Controls.Controls.Form1
{
    public class F1R2HhMembersControl : BaseControl, IF1R2HhMembersControl
    {
        private readonly IForm1Service _formService;
        private readonly IInterviewService _interviewService;

        private string _reportFilePath;
        private string _questionnaireTitle;

        public F1R2HhMembersControl(
            IForm1Service formService,
            IInterviewService interviewService,
            IQuestionnaireService questionnaireService,
            IHostingEnvironment hostEnv) : base(questionnaireService, hostEnv)
        {
            _formService = formService;
            _interviewService = interviewService;
        }

        public async Task<string> Execute(string questionnaireId, string region = null)
        {
            _reportFilePath = CreateReportFile("F1R2HhMembers");
            _questionnaireTitle = GetQuestionnaireTitle(questionnaireId);

            await Execute(questionnaireId, region, 0, 1000);

            return _reportFilePath;
        }

        private async Task Execute(string questionnaireId, string region = null, int offset = 0, int limit = 1000)
        {
            var interviews = _formService
                .GetF1R2Interviews(questionnaireId, offset, limit, region)
                .ToList();

            if (!(interviews.Count <= 0))
            {
                using (var file = DocX.Load(_reportFilePath))
                {
                    string error, hhCode, key;
                    bool isAgeValid;
                    foreach (var interview in interviews)
                    {
                        foreach (var questionData in interview.QuestionData)
                        {
                            error = "";
                            switch (questionData.QuestionCode)
                            {
                                // отношение к главе домохозяйства
                                case "f1r1q3":
                                    if (questionData.Answer == "1")
                                    {
                                        bool isMaritalStatusValid = await CheckMaritalStatus(interview.Id, questionData.QuestionSection);
                                        if (!isMaritalStatusValid)
                                            error = "Связь Муж/Жена";
                                    }
                                    break;
                                // дата рождения члена домохозяйства
                                case "f1r1q6":
                                    if (string.IsNullOrEmpty(questionData.Answer))
                                        error = "Дата рождения";
                                    break;
                                // лет на момент опроса члена домохозяйства
                                case "f1r1q7":
                                    try
                                    {
                                        isAgeValid = await CheckAge(interview.Id, questionData.QuestionSection, questionData.Answer);
                                        if (!isAgeValid)
                                            error = "Число полных лет на момент опроса";
                                    }
                                    catch (MemberAccessException) { continue; }
                                    break;
                            }

                            if (!string.IsNullOrEmpty(error))
                            {
                                hhCode = _interviewService.GetQuestionFirstAnswer(interview.Id, "hhCode");
                                key = _interviewService.GetInterviewKey(interview.Id);

                                file.InsertParagraph($"{FormString}: {_questionnaireTitle}.");
                                file.InsertParagraph($"{IdentifierString}: {key}.");
                                file.InsertParagraph($"{SectionString}: 2.");
                                file.InsertParagraph($"{HouseholdCodeString}: {hhCode}.");
                                file.InsertParagraph($"{ErrorString}: {error}.");
                                file.InsertParagraph();
                            }
                        }
                    }

                    file.Save();
                }

                await Execute(questionnaireId, region, offset += 1000);
            }
        }

        private async Task<bool> CheckAge(string interviewId, string section, string strAge)
        {
            string birthDateStr = await _formService.GetMemberBirthDate(interviewId, section);
            if (string.IsNullOrEmpty(birthDateStr))
                throw new MemberBirthDateIsNullException();

            DateTime birthDate = DateTime.Parse(birthDateStr);
            DateTime interviewDate = DateTime.Parse(
                await _formService.GetInterviewDate(interviewId));
            int age = int.Parse(strAge);

            return age == new DateTime(interviewDate.Subtract(birthDate).Ticks).Year - 1;
        }

        private async Task<bool> CheckMaritalStatus(string interviewId, string section)
        {
            int maritalStatus = int.Parse(await _formService.GetMemberMartialStatus(interviewId, section));
            int[] hasSpouseOptions = new int[3] { 1, 2, 4 };

            bool hasSpouse = await _formService.HasMemberSpouse(interviewId);

            if (hasSpouseOptions.Contains(maritalStatus))
            {
                return hasSpouse == true;
            } else
            {
                return hasSpouse == false;
            }
        }
    }
}
