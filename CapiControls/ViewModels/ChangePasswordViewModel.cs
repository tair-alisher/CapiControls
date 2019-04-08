using System.ComponentModel.DataAnnotations;

namespace CapiControls.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmNewPassword { get; set; }
    }
}
