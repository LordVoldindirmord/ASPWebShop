using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebShop.Domain.ViewModel.CategoryModel
{
    /// <summary>
    /// Модель для отображения категории
    /// </summary>
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Название категории")]
        public string Name { get; set; } = null!;

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Display(Name = "Количество товаров")]
        public int ProductCount { get; set; }
    }

    /// <summary>
    /// Модель для создания/редактирования категории
    /// </summary>
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 100 символов")]
        [Display(Name = "Название категории")]
        public string Name { get; set; } = null!;

        [Display(Name = "Описание категории")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string? Description { get; set; }
    }
}
