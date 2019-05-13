using CapiControls.BLL.DTO.Base;

namespace CapiControls.BLL.DTO.Account
{
    public class ChangePasswordDTO : BaseDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Secret { get; set; }
    }
}
