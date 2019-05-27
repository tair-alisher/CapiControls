using CapiControls.BLL.DTO;
using CapiControls.BLL.Exceptions;
using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Interfaces.Form1;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Hosting;
using Novacode;
using System;
using System.Collections.Generic;
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

        private async Task CheckInterviews(List<InterviewDTO> interviews, IterationData iterationData)
        {
            if (!(interviews.Count <= 0))
            {
                using (var file = DocX.Load(_reportFilePath))
                {
                    string memberName, error;
                    bool isAgeValid, isHeadmanDroppedOut;
                    foreach (var interview in interviews)
                    {
                        foreach (var questionData in interview.QuestionData)
                        {
                            memberName = await _uow.Form1Repository.GetCurrentMemberName(interview.Id, questionData.QuestionSection.Split('_')[1]);
                            error = null;
                            isHeadmanDroppedOut = await _uow.Form1Repository.IsHeadmanDroppedOut(interview.Id, questionData.QuestionSection);
                            switch (questionData.QuestionCode)
                            {
                                // отношение к главе домохозяйства
                                case "f1r1q3":
                                    if (questionData.Answer == "1")
                                    {
                                        try
                                        {
                                            bool isMaritalStatusValid = await CheckMaritalStatus(interview.Id, questionData.QuestionSection);
                                            if (!isMaritalStatusValid)
                                                error = $"{memberName}. Связь Муж/Жена";
                                        }
                                        catch (MaritalStatusNotAnsweredException)
                                        {
                                            error = $"{memberName}. Семейное положение без ответа";
                                        }
                                    }
                                    break;
                                // лет на момент опроса члена домохозяйства
                                // дата рождения - заполняемость
                                // дата проведения интервью - заполняемость
                                case "f1r1q7":
                                    if (!isHeadmanDroppedOut)
                                    {
                                        try
                                        {
                                            isAgeValid = await CheckAge(interview.Id, questionData.QuestionSection, questionData.Answer);
                                            if (!isAgeValid)
                                                error = $"{memberName}. Число полных лет на момент опроса";
                                        }
                                        catch (MemberDroppedOutException) { break; }
                                        catch (MemberBirthDateIsNullException ex) { error = $"{memberName}. {ex.Message}"; }
                                        catch (InterviewDateIsNullException ex) { error = $"{memberName}. {ex.Message}"; }
                                    }
                                    break;
                                // проживает ли в домохозяйстве
                                case "f1r1q4":
                                    // break if member is headman and is dropped out
                                    if (isHeadmanDroppedOut)
                                        break;

                                    // is presence not valid
                                    if (!(await CheckPresenceInHousehold(interview.Id, questionData.QuestionSection, questionData.Answer)))
                                        error = $"{memberName}. Является ли проживающим/причина отсутствия";
                                    break;
                            } // switch end

                            // write the error to the file
                            if (!string.IsNullOrEmpty(error))
                                WriteErrorToFile(file, interview.Id, error);
                        }
                    }

                    // save the file
                    file.Save();
                }

                await Execute(iterationData.QuestionnaireId, iterationData.Region, iterationData.Offset += 1000);
            }
        }

        private async Task Execute(string questionnaireId, string region = null, int offset = 0, int limit = 1000)
        {
            var rawInterviewsData = _uow.Form1Repository
                .GetF1R2Interviews(questionnaireId, offset, limit, region)
                .ToList();

            var iterationData = new IterationData
            {
                QuestionnaireId = questionnaireId,
                Region = region,
                Offset = offset
            };
            var interviews = FormInterviews(iterationData);
            await CheckInterviews(interviews, iterationData);
        }

        private List<InterviewDTO> FormInterviews(IterationData iterationData)
        {
            return _interviewService.CollectInterviews(
                _uow.Form1Repository.GetF1R2Interviews(iterationData.QuestionnaireId, iterationData.Offset, iterationData.Limit, iterationData.Region));
        }

        private void WriteErrorToFile(DocX file, string interviewId, string error)
        {
            string hhCode = _interviewService.GetQuestionFirstAnswer(interviewId, "hhCode");
            string key = _interviewService.GetInterviewKey(interviewId);

            file.InsertParagraph($"{FormString}: {_questionnaireTitle}.");
            file.InsertParagraph($"{IdentifierString}: {key}.");
            file.InsertParagraph($"{SectionString}: 2.");
            file.InsertParagraph($"{HouseholdCodeString}: {hhCode}.");
            file.InsertParagraph($"{ErrorString}: {error}.");
            file.InsertParagraph();
        }

        private async Task<bool> CheckAge(string interviewId, string section, string strAge)
        {
            string birthDateStr = await _uow.Form1Repository.GetMemberBirthDate(interviewId, section);
            if (string.IsNullOrEmpty(birthDateStr))
            {
                bool isDroppedOut = await _uow.Form1Repository.IsMemberDroppedOut(interviewId, section);
                if (isDroppedOut)
                    throw new MemberDroppedOutException();
                else
                    throw new MemberBirthDateIsNullException();
            }

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
            string result = await _uow.Form1Repository.GetMemberMaritalStatus(interviewId, section);
            if (string.IsNullOrEmpty(result))
                throw new MaritalStatusNotAnsweredException();

            int maritalStatus = int.Parse(result);

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

    internal class IterationData
    {
        public string QuestionnaireId { get; set; }
        public string Region { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
