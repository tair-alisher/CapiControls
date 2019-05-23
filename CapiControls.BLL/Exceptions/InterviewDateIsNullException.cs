using System;

namespace CapiControls.BLL.Exceptions
{
    public class InterviewDateIsNullException : Exception
    {
        public InterviewDateIsNullException() : base("Не указана фактическая дата проведения интервью") { }
    }
}
