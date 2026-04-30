using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebShop.Domain.ViewModel.SellerModel
{
    /// <summary>
    /// Модель для отображения информации о продавце
    /// </summary>
    public class SellerViewModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Display(Name = "Название магазина")]
        public string StoreName { get; set; } = null!;

        [Display(Name = "Описание магазина")]
        public string? Description { get; set; }

        [Display(Name = "Имя продавца")]
        public string SellerFirstName { get; set; } = null!;

        [Display(Name = "Фамилия продавца")]
        public string SellerLastName { get; set; } = null!;

        [Display(Name = "Email продавца")]
        public string SellerEmail { get; set; } = null!;

        [Display(Name = "Количество товаров")]
        public int ProductCount { get; set; }
    }

    /// <summary>
    /// Модель для создания/редактирования профиля продавца
    /// </summary>
    public class CreateSellerViewModel
    {
        [Required(ErrorMessage = "Название магазина обязательно")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Название магазина должно быть от 3 до 200 символов")]
        [Display(Name = "Название магазина")]
        public string StoreName { get; set; } = null!;

        [Display(Name = "Описание магазина")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "Описание не должно превышать 1000 символов")]
        public string? Description { get; set; }
    }
}
