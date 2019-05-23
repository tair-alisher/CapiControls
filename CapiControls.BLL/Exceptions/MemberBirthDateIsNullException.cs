using System;

namespace CapiControls.BLL.Exceptions
{
    public class MemberBirthDateIsNullException : Exception
    {
        public MemberBirthDateIsNullException() : base("Не указана дата рождения члена домохозяйства") {}
    }
}
