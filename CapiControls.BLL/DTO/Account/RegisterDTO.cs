using CapiControls.BLL.DTO.Base;

namespace CapiControls.BLL.DTO.Account
{
    public class RegisterDTO : BaseDTO
    {
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Secret { get; set; }
        public string[] Roles { get; set; }
    }
}
