using CapiControls.BLL.DTO.Base;

namespace CapiControls.BLL.DTO.Account
{
    public class LoginDTO : BaseDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Secret { get; set; }
    }
}
