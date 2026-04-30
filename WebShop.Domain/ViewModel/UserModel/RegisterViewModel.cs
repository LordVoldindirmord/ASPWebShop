using System.ComponentModel.DataAnnotations;

namespace WebShop.Domain.ViewModel.UserModel
{
    /// <summary>
    /// Модель для регистрации нового пользователя
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно быть от 2 до 100 символов")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Фамилия должна быть от 2 до 100 символов")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [EmailAddress(ErrorMessage = "Введите корректный Email адрес")]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
