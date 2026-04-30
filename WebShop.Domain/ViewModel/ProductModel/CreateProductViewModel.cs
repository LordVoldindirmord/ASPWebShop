using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebShop.Domain.ViewModel.ProductModel
{
    /// <summary>
    /// Модель для создания нового товара
    /// Изображения передаются как массив байтов
    /// </summary>
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 200 символов")]
        [Display(Name = "Название товара")]
        public string Name { get; set; } = null!;

        [Display(Name = "Описание товара")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, 9999999.99, ErrorMessage = "Цена должна быть от 0.01 до 9 999 999.99")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Количество на складе обязательно")]
        [Range(0, 99999, ErrorMessage = "Количество должно быть от 0 до 99 999")]
        [Display(Name = "Количество на складе")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Главное изображение товара (массив байтов)
        /// </summary>
        [Display(Name = "Главное изображение")]
        public byte[]? MainImage { get; set; }

        /// <summary>
        /// Имя файла главного изображения (для определения расширения)
        /// </summary>
        public string? MainImageFileName { get; set; }

        /// <summary>
        /// Дополнительные изображения товара
        /// </summary>
        [Display(Name = "Дополнительные изображения")]
        public List<ProductImageData>? AdditionalImages { get; set; }
    }

    /// <summary>
    /// Данные одного дополнительного изображения
    /// </summary>
    public class ProductImageData
    {
        /// <summary>
        /// Содержимое файла изображения
        /// </summary>
        public byte[] ImageBytes { get; set; } = null!;

        /// <summary>
        /// Имя файла с расширением
        /// </summary>
        public string FileName { get; set; } = null!;
    }

    /// <summary>
    /// Модель для редактирования товара
    /// </summary>
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(200, MinimumLength = 3)]
        [Display(Name = "Название товара")]
        public string Name { get; set; } = null!;

        [Display(Name = "Описание товара")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, 9999999.99)]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Количество на складе обязательно")]
        [Range(0, 99999)]
        [Display(Name = "Количество на складе")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Текущий URL главного изображения
        /// </summary>
        public string? ExistingMainImageUrl { get; set; }

        /// <summary>
        /// Новое главное изображение (если нужно заменить)
        /// </summary>
        public byte[]? NewMainImage { get; set; }
        public string? NewMainImageFileName { get; set; }

        /// <summary>
        /// Текущие URL дополнительных изображений
        /// </summary>
        public List<string>? ExistingAdditionalImages { get; set; }

        /// <summary>
        /// ID изображений для удаления
        /// </summary>
        public List<int>? ImagesToDelete { get; set; }

        /// <summary>
        /// Новые дополнительные изображения
        /// </summary>
        public List<ProductImageData>? NewAdditionalImages { get; set; }
    }
}