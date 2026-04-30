using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebShop.Domain.ViewModel.ProductModel
{
    /// <summary>
    /// Модель для отображения товара в каталоге
    /// </summary>
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; } = null!;

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Display(Name = "Цена")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        [Display(Name = "В наличии")]
        public int StockQuantity { get; set; }

        [Display(Name = "Главное изображение")]
        public string? MainImageUrl { get; set; }

        [Display(Name = "Категория")]
        public string CategoryName { get; set; } = null!;

        [Display(Name = "Продавец")]
        public string SellerName { get; set; } = null!;

        [Display(Name = "Активен")]
        public bool IsActive { get; set; }

        [Display(Name = "Дата добавления")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Дополнительные изображения")]
        public List<string> Images { get; set; } = new();
    }

    /// <summary>
    /// Модель для детального просмотра товара
    /// </summary>
    public class ProductDetailViewModel : ProductViewModel
    {
        public int CategoryId { get; set; }
        public int SellerId { get; set; }

        [Display(Name = "Описание продавца")]
        public string? SellerDescription { get; set; }
    }
}
