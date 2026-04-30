using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebShop.Domain.ViewModel.OrderModel
{
    /// <summary>
    /// Модель для отображения заказа
    /// </summary>
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Дата заказа")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "Сумма заказа")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Статус")]
        public string Status { get; set; } = null!;

        [Display(Name = "Адрес доставки")]
        public string ShippingAddress { get; set; } = null!;

        [Display(Name = "Покупатель")]
        public string CustomerName { get; set; } = null!;

        [Display(Name = "Email покупателя")]
        public string CustomerEmail { get; set; } = null!;

        [Display(Name = "Товары в заказе")]
        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    /// <summary>
    /// Модель для отображения позиции в заказе
    /// </summary>
    public class OrderItemViewModel
    {
        public int? ProductId { get; set; }

        [Display(Name = "Название товара")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Цена")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        [Display(Name = "Сумма")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalPrice => Price * Quantity;
    }
}
