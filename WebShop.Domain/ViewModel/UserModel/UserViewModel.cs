using System.ComponentModel.DataAnnotations;
using WebShop.Domain.Enum;

namespace WebShop.Domain.ViewModel.UserModel
{
    /// <summary>
    /// Модель для отображения информации о пользователе
    /// </summary>
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно быть от 2 до 100 символов")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Фамилия должна быть от 2 до 100 символов")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Некорректный формат телефона")]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Роль")]
        public UserRole Role { get; set; }

        [Display(Name = "Дата регистрации")]
        public DateTime? CreatedAt { get; set; }
    }
}
