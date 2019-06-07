using CapiControls.BLL.DTO;
using CapiControls.BLL.Exceptions;
using CapiControls.BLL.Interfaces;
using CapiControls.Controls.Common;
using CapiControls.Controls.Interfaces.Form1;
using CapiControls.DAL.Common;
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
        protected override string SectionNumber
        {
            get { return "2"; }
        }

        public F1R2HhMembersControl(
            IRemoteUnitOfWork uow,
            IInterviewService interviewService,
            IQuestionnaireService questionnaireService,
            IHostingEnvironment hostEnv) : base(uow, questionnaireService, interviewService, hostEnv)
        { }

        public async Task<string> Execute(string questionnaireId, string region = null)
        {
            _reportFilePath = CreateReportFile("F1R2HhMembers");
            _questionnaireTitle = GetQuestionnaireTitle(questionnaireId);

            await Execute(new QueryParams
            {
                QuestionnaireId = questionnaireId,
                Region = region
            });

            return _reportFilePath;
        }

        private async Task Execute(QueryParams parameters)
        {
            var interviews = base.InterviewService.CollectInterviews(
                Uow.Form1Repository.GetF1R2Interviews(parameters)
            );

            if (interviews.Count <= 0)
                return;

            using (var file = DocX.Load(_reportFilePath))
            {
                await CheckInterviews(interviews, file);
                file.Save();
            }

            parameters.Offset += 1000;
            await Execute(parameters);
        }

        private async Task CheckInterviews(List<InterviewDTO> interviews, DocX file)
        {
            foreach (var interview in interviews)
                await CheckInterview(interview, file);
        }

        private async Task CheckInterview(InterviewDTO interview, DocX file)
        {
            string error = null;
            foreach (var questionData in interview.QuestionData)
            {
                error = await CheckAnswer(interview, questionData);

                if (!string.IsNullOrEmpty(error))
                    base.WriteErrorToFile(file, interview.Id, error, SectionNumber);
            }
        }

        private async Task<string> CheckAnswer(InterviewDTO interview, QuestionDataDTO questionData)
        {
            string error = "";
            string memberName = await Uow.Form1Repository.GetCurrentMemberName(interview.Id, questionData.QuestionSection.Split('_')[1]);
            bool isHeadmanDroppedOut = await Uow.Form1Repository.IsHeadmanDroppedOut(interview.Id, questionData.QuestionSection);

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
            string result = await Uow.Form1Repository.GetMemberMaritalStatus(interviewId, section);
            if (string.IsNullOrEmpty(result))
                throw new MaritalStatusNotAnsweredException();

            int maritalStatus = int.Parse(result);

            int[] hasSpouseOptions = new int[3] { 1, 2, 4 };

            bool hasSpouse = await Uow.Form1Repository.HasMemberSpouse(interviewId);

            // если у главы есть супруг, должна быть информаци я о нем (супруге)
            return hasSpouseOptions.Contains(maritalStatus) ? hasSpouse : !hasSpouse;
        }

        private async Task<string> CheckAge(string interviewId, string memberName, string section, string strAge)
        {
            if (await Uow.Form1Repository.IsMemberDroppedOut(interviewId, section))
                return string.Empty;

            var birthDateStr = await Uow.Form1Repository.GetMemberBirthDate(interviewId, section);
            if (string.IsNullOrEmpty(birthDateStr))
                return $"{memberName}. Не указана дата рождения члена домохозяйства.\n\tНевозможно проверить число полных лет на момент опроса";

            var interviewDateStr = await Uow.Form1Repository.GetInterviewDate(interviewId);
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
            bool isMemberAbsenceReasonAnswered = await Uow.Form1Repository.IsMemberAbsenceReasonAnswered(interviewId, section);

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
