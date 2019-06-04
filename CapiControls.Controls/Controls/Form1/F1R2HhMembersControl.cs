using CapiControls.BLL.DTO;
using CapiControls.BLL.Exceptions;
using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Common;
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

            var iterationData = new IterationData
            {
                QuestionnaireId = questionnaireId,
                Region = region,
                Offset = offset,
                Limit = limit
            };

            await CheckInterviews(iterationData);
        }

        private async Task CheckInterviews(IterationData iterationData)
        {
            var interviews = FormInterviews(iterationData);

            if (interviews.Count <= 0)
                return;

            using (var file = DocX.Load(_reportFilePath))
            {
                foreach (var interview in interviews)
                    await CheckInterview(interview, file);

                file.Save();
            }

            await Execute(iterationData.QuestionnaireId, iterationData.Region, iterationData.Offset += 1000);
        }

        private List<InterviewDTO> FormInterviews(IterationData iterationData)
        {
            return _interviewService.CollectInterviews(
                _uow.Form1Repository.GetF1R2Interviews(iterationData.QuestionnaireId, iterationData.Offset, iterationData.Limit, iterationData.Region));
        }

        private async Task CheckInterview(InterviewDTO interview, DocX file)
        {
            string error = null;
            foreach (var questionData in interview.QuestionData)
            {
                error = await CheckAnswer(interview, questionData);

                if (!string.IsNullOrEmpty(error))
                    WriteErrorToFile(file, interview.Id, error);
            }
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

        private async Task<string> CheckAnswer(InterviewDTO interview, QuestionDataDTO questionData)
        {
            string error = "";
            string memberName = await _uow.Form1Repository.GetCurrentMemberName(interview.Id, questionData.QuestionSection.Split('_')[1]);
            bool isHeadmanDroppedOut = await _uow.Form1Repository.IsHeadmanDroppedOut(interview.Id, questionData.QuestionSection);

            switch (questionData.QuestionCode)
            {
                case "f1r1q3":
                    if (questionData.Answer == "1")
                        error = await CheckHeadMaritalStatus(interview.Id, memberName, questionData.QuestionSection);
                    break;
                // лет на момент опроса члена домохозяйства
                // дата рождения - заполняемость
                // дата проведения интервью - заполняемость
                case "f1r1q7":
                    if (!isHeadmanDroppedOut)
                        error = await CheckAge(interview.Id, memberName, questionData.QuestionSection, questionData.Answer);
                    break;
                // проживает ли в домохозяйстве
                case "f1r1q4":
                    // проживает ли человек в домохозяйстве/причина отсутствия
                    if (!isHeadmanDroppedOut)
                        error = await CheckPresenceInHousehold(interview.Id, memberName, questionData.QuestionSection, questionData.Answer);
                    break;
            }

            return error;
        }
        private async Task<string> CheckHeadMaritalStatus(string interviewId, string memberName, string questionSection)
        {
            var error = "";
            try
            {
                if (!(await IsMaritalStatusValid(interviewId, questionSection)))
                    error = $"{memberName}. Связь Муж/Жена";
            }
            catch (MaritalStatusNotAnsweredException)
            {
                error = $"{memberName}. Семейное положение без ответа";
            }

            return error;
        }

        private async Task<bool> IsMaritalStatusValid(string interviewId, string section)
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

        private async Task<string> CheckAge(string interviewId, string memberName, string section, string strAge)
        {
            if (await _uow.Form1Repository.IsMemberDroppedOut(interviewId, section))
                return string.Empty;

            var birthDateStr = await _uow.Form1Repository.GetMemberBirthDate(interviewId, section);
            if (string.IsNullOrEmpty(birthDateStr))
                return $"{memberName}. Не указана дата рождения члена домохозяйства.\n\tНевозможно проверить число полных лет на момент опроса";

            var interviewDateStr = await _uow.Form1Repository.GetInterviewDate(interviewId);
            if (string.IsNullOrEmpty(interviewDateStr))
                return $"{memberName}. Не указана фактическая дата проведения интервью.\n\tНевозможно проверить число полных лет на момент опроса";

            var birthDate = DateTime.Parse(birthDateStr);
            var interviewDate = DateTime.Parse(interviewDateStr);

            if (int.Parse(strAge) == new DateTime(interviewDate.Subtract(birthDate).Ticks).Year - 1)
                return string.Empty;
            else
                return $"{memberName}. Число полных лет на момент опроса";
        }

        private async Task<string> CheckPresenceInHousehold(string interviewId, string memberName, string section, string answer)
        {
            string error = $"{memberName}. Является ли проживающим/причина отсутствия";
            bool isMemberAbsenceReasonAnswered = await _uow.Form1Repository.IsMemberAbsenceReasonAnswered(interviewId, section);

            if (answer == "1") // если человек проживает в домохозяйстве
            {
                if (isMemberAbsenceReasonAnswered) // но при этом указана причина отсутствия
                    return error;
            }
            else // если человек НЕ проживает в домохозяйстве
            {
                if (!isMemberAbsenceReasonAnswered) // но причина отсутвия не указана
                    return error;
            }

            return string.Empty;
        }
    }
}
