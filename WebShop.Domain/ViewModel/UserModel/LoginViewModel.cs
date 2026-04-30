using System.ComponentModel.DataAnnotations;

namespace WebShop.Domain.ViewModel.UserModel
{
    /// <summary>
    /// Модель для входа в систему
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [EmailAddress(ErrorMessage = "Введите корректный Email адрес")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
