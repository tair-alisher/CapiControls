using CapiControls.BLL.Exceptions;
using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Interfaces.Form1;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CapiControls.Controls.Controls.Form1
{
    public class F1R2HhMembersControl : BaseControl, IF1R2HhMembersControl
    {
        private readonly IRemoteUnitOfWork _uow;
        private readonly IInterviewService _interviewService;

        private string _reportFilePath;
        private string _questionnaireTitle;

        public F1R2HhMembersControl(
            IRemoteUnitOfWork uow,
            IInterviewService interviewService,
            IQuestionnaireService questionnaireService,
            IHostingEnvironment hostEnv) : base(questionnaireService, hostEnv)
        {
            _uow = uow;
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
            var rawInterviewsData = _uow.Form1Repository
                .GetF1R2Interviews(questionnaireId, offset, limit, region)
                .ToList();
            var interviews = _interviewService.CollectInterviews(rawInterviewsData);

            if (!(interviews.Count <= 0))
            {
                using (var file = DocX.Load(_reportFilePath))
                {
                    string error, hhCode, key;
                    bool isAgeValid, isPresenceValid;
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
                                // лет на момент опроса члена домохозяйства
                                // дата рождения - заполняемость
                                // дата проведения интервью - заполняемость
                                case "f1r1q7":
                                    try
                                    {
                                        isAgeValid = await CheckAge(interview.Id, questionData.QuestionSection, questionData.Answer);
                                        if (!isAgeValid)
                                            error = "Число полных лет на момент опроса";
                                    }
                                    catch (MemberBirthDateIsNullException)
                                    {
                                        error = "Не указана дата рождения члена домохозяйства";
                                    }
                                    catch (InterviewDateIsNullException)
                                    {
                                        error = "Не указана фактическая дата проведения интервью";
                                    }
                                    break;
                                // проживает ли в домохозяйстве
                                case "f1r1q4":
                                    isPresenceValid = await CheckPresenceInHousehold(interview.Id, questionData.QuestionSection, questionData.Answer);
                                    if (!isPresenceValid)
                                        error = "Является ли проживающим/причина отсутствия";
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
            string birthDateStr = await _uow.Form1Repository.GetMemberBirthDate(interviewId, section);
            if (string.IsNullOrEmpty(birthDateStr))
                throw new MemberBirthDateIsNullException();

            string interviewDateStr = await _uow.Form1Repository.GetInterviewDate(interviewId);
            if (string.IsNullOrEmpty(interviewDateStr))
                throw new InterviewDateIsNullException();

            DateTime birthDate = DateTime.Parse(birthDateStr);
            DateTime interviewDate = DateTime.Parse(interviewDateStr);
            int age = int.Parse(strAge);

            return age == new DateTime(interviewDate.Subtract(birthDate).Ticks).Year - 1;
        }

        private async Task<bool> CheckMaritalStatus(string interviewId, string section)
        {
            int maritalStatus = int.Parse(await _uow.Form1Repository.GetMemberMaritalStatus(interviewId, section));
            int[] hasSpouseOptions = new int[3] { 1, 2, 4 };

            bool hasSpouse = await _uow.Form1Repository.HasMemberSpouse(interviewId);

            // если у главы есть супруг, должна быть информаци я о нем (супруге)
            return hasSpouseOptions.Contains(maritalStatus) ? hasSpouse : !hasSpouse;
        }
        
        private async Task<bool> CheckPresenceInHousehold(string interviewId, string section, string answer)
        {
            bool isMemberAbsenceReasonAnswered = await _uow.Form1Repository.IsMemberAbsenceReasonAnswered(interviewId, section);

            // если человек проживает в домохозяйстве, причина отсутствия не должна указываться
            // иначе, необходимо указать причину отсутствия
            return answer == "1" ? !isMemberAbsenceReasonAnswered : isMemberAbsenceReasonAnswered;
        }
    }
}
