using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebShop.Domain.ViewModel.CartModel
{
    /// <summary>
    /// Модель для отображения товара в корзине
    /// </summary>
    public class CartItemViewModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Display(Name = "Название товара")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Цена за единицу")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        [Display(Name = "Сумма")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalPrice => Price * Quantity;

        [Display(Name = "Изображение")]
        public string? MainImageUrl { get; set; }

        [Display(Name = "В наличии")]
        public int StockQuantity { get; set; }

        [Display(Name = "Категория")]
        public string CategoryName { get; set; } = null!;
    }

    /// <summary>
    /// Модель для добавления товара в корзину
    /// </summary>
    public class AddToCartViewModel
    {
        [Required(ErrorMessage = "ID товара обязателен")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Укажите количество")]
        [Range(1, 100, ErrorMessage = "Количество должно быть от 1 до 100")]
        [Display(Name = "Количество")]
        public int Quantity { get; set; } = 1;
    }

    /// <summary>
    /// Модель для отображения всей корзины
    /// </summary>
    public class CartViewModel
    {
        [Display(Name = "Товары в корзине")]
        public List<CartItemViewModel> Items { get; set; } = new();

        [Display(Name = "Общая сумма")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Количество товаров")]
        public int ItemsCount { get; set; }
    }
}
